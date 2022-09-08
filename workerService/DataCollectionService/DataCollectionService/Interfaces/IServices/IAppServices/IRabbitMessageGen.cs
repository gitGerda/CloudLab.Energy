using HangfireJobsToRabbitLibrary.Models;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using KzmpEnergyIndicationsLibrary.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Interfaces.IServices.IAppServices
{
    public interface IRabbitMessageGen
    {

        /// <summary>
        /// Публикация сообщения
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool PublishMessageToRabbit<T>(T message);
        /*        /// <summary>
                /// Публикация показаний энергий
                /// </summary>
                /// <param name="message"></param>
                /// <returns></returns>
                public bool PublishEnergyIndicationsMessage(EnergyRecordResponse message);
                /// <summary>
                /// Публикация показаний профиля мощности
                /// </summary>
                /// <param name="message"></param>
                /// <returns></returns>
                public bool PublishPowerProfilesMessage(PowerProfilesBrokerMessage message);
                /// <summary>
                /// Функция публикации сообщения типа SheduleLog
                /// </summary>
                /// <param name="shedule_log"></param>
                /// <returns></returns>
                public bool PublishSheduleLogMessage(SheduleLog shedule_log);
                /// <summary>
                /// Публикация проваленного задания
                /// </summary>
                /// <param name="message"></param>
                /// <returns></returns>
                public bool PublishFailedBrokerTaskTypeMessage(BrokerTaskMessage message);
                /// <summary>
                /// Создание объекта типа SheduleLog
                /// </summary>
                /// <param name="shedule_id"></param>
                /// <param name="status"></param>
                /// <param name="description"></param>
                /// <returns></returns>
        */
        public SheduleLog CreateSheduleLog(int shedule_id, string status, string description, DateTime? date);

    }
}
