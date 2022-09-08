using Hangfire;
using HangfireJobsToRabbitLibrary.Models;
using Newtonsoft.Json;

namespace HangfireJobsToRabbitLibrary.Jobs.IndicationsReading
{
    public class IndicationsReadingJob : IIndicationsReadingJob
    {
        IHangfireRabbitJobs _hangfire_rabbit_jobs;
        HttpClient _http_client;
        public IndicationsReadingJob(IHangfireRabbitJobs hangfire_rabbit_jobs, HttpClient httpClient)
        {
            _hangfire_rabbit_jobs = hangfire_rabbit_jobs;
            _http_client = httpClient;
        }
        public async Task<bool> PushJobToRabbit(JobCreateSettings settings, List<BrokerTaskMessage> message)
        {
            switch (settings.periodicity)
            {
                case "every day":
                    /*                    settings.periodicity = Cron.Daily();
                    */
                    settings.periodicity = Cron.Daily(10, 0);//every day at 10:00
                    break;
                case "every week":
                    /*                    settings.periodicity = Cron.Weekly();
                    */
                    settings.periodicity = Cron.Weekly(DayOfWeek.Monday, 10, 0);//every monday at 10:00
                    break;
                case "every month":
                    /*                    settings.periodicity = Cron.Monthly();
                    */
                    settings.periodicity = Cron.Monthly(1, 10, 0);//At 10:00 AM, on day 1 of the month
                    break;
                default:
                    throw new Exception("Invalid periodicity");
            }

            foreach (var brokerTaskMessage in message)
            {
                string _request = settings.last_datetime_request + brokerTaskMessage.meter_address;
                brokerTaskMessage.last_indication_datetime = await _hangfire_rabbit_jobs.GetLastIndicationDateTime(_http_client,
                    _request);
                if (brokerTaskMessage.last_indication_datetime == null)
                    throw new Exception("Ошибка запроса даты и времени последнего измерения счётчика");

                if (brokerTaskMessage.last_indication_datetime == "")
                {
                    DateTime _start_date;
                    if (!DateTime.TryParse(brokerTaskMessage.start_date, out _start_date))
                        throw new Exception("Ошибка при конвертации строки start_date объекта JobSettings в форматы даты");
                    brokerTaskMessage.last_indication_datetime = $"{_start_date.ToShortDateString()} {_start_date.Hour}:00:00";
                }
            }

            var result = await _hangfire_rabbit_jobs.ReccuringJobAddOrUpdate(settings, message);

            return result;
        }
    }
}
