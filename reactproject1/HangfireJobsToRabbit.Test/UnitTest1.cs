using HangfireJobsToRabbit.Jobs.IndicationsReading;
using HangfireJobsToRabbit.Models;
using Moq;

namespace HangfireJobsToRabbit.Test
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("every day")]
        [InlineData("every week")]
        [InlineData("every month")]
        public async void PushToRabbit_ValidSettingsPeriodicity_TrueResult(string periodicity)
        {
            //arrange
            var _settings = new JobCreateSettings()
            {
                job_id = "1",
                cron_periodicity = periodicity
            };
            var _job_mocked = new Mock<IndicationsReadingJob>();
            _job_mocked.Setup(ex => ex.HangfireRecJobAddOrUpdate(_settings.job_id, "", ""));


            //act
            var result = await _job_mocked.Object.PushJobToRabbit(settings: _settings,
                message: new List<BrokerTaskMessage>());

            //assert
            Assert.True(result);
        }
    }
}