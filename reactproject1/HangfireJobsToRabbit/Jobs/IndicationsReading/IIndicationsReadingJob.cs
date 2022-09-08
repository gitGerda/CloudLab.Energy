using HangfireJobsToRabbitLibrary.Models;

namespace HangfireJobsToRabbitLibrary.Jobs.IndicationsReading
{
    public interface IIndicationsReadingJob
    {
        /// <summary>
        /// Функция, которая помещает задание (message) для точки опроса (communic_point) в очередь брокера.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="message"></param>
        Task<bool> PushJobToRabbit(JobCreateSettings settings, List<BrokerTaskMessage> message);
    }
}
