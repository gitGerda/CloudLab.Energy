using Hangfire;
using HangfireJobsToRabbitLibrary.Jobs.IndicationsReading;
using HangfireJobsToRabbitLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace kzmpCloudAPI.Tests.LibrariesTest.HangfireJobsToRabbit
{
    public class IndicationReadingJobTest
    {
        [Theory]
        [InlineData("every day")]
        [InlineData("every week")]
        [InlineData("every month")]
        public async void PushToRabbit_ValidSettingsPeriodicity_TrueResult(string periodicity)
        {
            //arrange
            string _cron_periodicity = ConvertToCronPeriodicity(periodicity);

            var _settings = new JobCreateSettings()
            {
                job_id = "1",
                periodicity = periodicity
            };
            var _ihangfire_rabbit_job_mock = new Mock<IHangfireRabbitJobs>();
            _ihangfire_rabbit_job_mock.Setup(ex => ex.ReccuringJobAddOrUpdate(_settings, new List<BrokerTaskMessage>())).Returns(Task.FromResult(true));
            var _http_client_mock = new Mock<HttpClient>();

            var _job_mocked = new IndicationsReadingJob(_ihangfire_rabbit_job_mock.Object, _http_client_mock.Object);

            //act
            var result = await _job_mocked.PushJobToRabbit(settings: _settings,
                message: new List<BrokerTaskMessage>());

            //assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("http://localhost/", "every day")]
        public async void PushToRabbit_NullLastReadingDate_ExceptionResult(string request, string periodicity)
        {
            //arrange
            var _http_client_mock = new Mock<HttpClient>();
            var _hangfire_rabbit_jobs_mock = new Mock<IHangfireRabbitJobs>();
            _hangfire_rabbit_jobs_mock.Setup(ex => ex.GetLastIndicationDateTime(_http_client_mock.Object, request))
                .Returns(() => null);
            var _indication_reading_job = new IndicationsReadingJob(_hangfire_rabbit_jobs_mock.Object, _http_client_mock.Object);
            var _job_settings = new JobCreateSettings()
            {
                periodicity = periodicity
            };
            var _cron_periodicity = ConvertToCronPeriodicity(periodicity);
            //assert
            await Assert.ThrowsAsync<Exception>(async () => await _indication_reading_job.PushJobToRabbit(_job_settings, new List<BrokerTaskMessage>() { new BrokerTaskMessage() { } }));
        }

        [Theory]
        [InlineData("2021-09-24 23:15:00", "http://localhost", "every day", "24.09.2021 23:00:00")]
        [InlineData("22-06-2022 11:37:00", "http://localhost", "every day", "22.06.2022 11:00:00")]
        public async void PushToRabbit_EmptyLastReadingDate_LastReadingDateEqualStartDateWithZeroMinutes(string start_date, string request, string periodicity, string result_value)
        {
            //arrange
            var _job_settings = new JobCreateSettings()
            {
                periodicity = periodicity,
                last_datetime_request = request
            };
            var _message = new List<BrokerTaskMessage>() {
                new BrokerTaskMessage(){
                    start_date=start_date
                }
            };

            var _http_client_mock = new Mock<HttpClient>();
            var _hangfire_rabbit_jobs_mock = new Mock<IHangfireRabbitJobs>();
            _hangfire_rabbit_jobs_mock.Setup(ex => ex.GetLastIndicationDateTime(_http_client_mock.Object, request))
                .Returns(() => Task.FromResult(""));
            _hangfire_rabbit_jobs_mock.Setup(ex => ex.ReccuringJobAddOrUpdate(_job_settings, _message))
                .Returns(Task.FromResult(true));

            var _indication_reading_job = new IndicationsReadingJob(_hangfire_rabbit_jobs_mock.Object, _http_client_mock.Object);

            //act
            var result = await _indication_reading_job.PushJobToRabbit(_job_settings, _message);

            //assert
            foreach (var _broker_task_mes in _message)
            {
                Assert.Equal(result_value, _broker_task_mes.last_indication_datetime);
            }
        }
        private string ConvertToCronPeriodicity(string periodicity)
        {
            string _cron_periodicity = "";
            switch (periodicity)
            {
                case "every day":
                    _cron_periodicity = Cron.Daily();
                    break;
                case "every week":
                    _cron_periodicity = Cron.Weekly();
                    break;
                case "every month":
                    _cron_periodicity = Cron.Monthly();
                    break;
                default:
                    break;
            }
            return _cron_periodicity;

        }

    }
}