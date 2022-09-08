using HangfireJobsToRabbitLibrary.Models;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using static kzmpCloudAPI.Components.General.AppConsts;

namespace kzmpCloudAPI.Interfaces
{
    public interface IRabbitMessagesHandler
    {
        /// <summary>
        /// Десериализация сообщения, полученного от брокера
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="broker_message"></param>
        /// <returns></returns>
        public T DeserializeBrokerMessage<T>(string broker_message);
        /// <summary>
        /// Конвертировать сообщение от брокера в строку
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string BrokerMessageToString(byte[] message);
        /// <summary>
        /// Определение типа сообщения полученного от брокера
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public broker_messages_types? GetTypeOfBrokerMessage(string message);
        /// <summary>
        /// Функция обработки сообщения типа SheduleLog
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HandleSheduleLogTypeMessage(SheduleLog message);
        /// <summary>
        /// Функция обработки сообщения типа PowerProfilesBrokerMessage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HandlePowerProfilesTypeMessage(PowerProfilesBrokerMessage message);
        /// <summary>
        /// Функция обработки сообщения типа BrokerTaskMessage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HandleBrokerTaskTypeMessage(BrokerTaskMessage message);
        /// <summary>
        /// Функция обработки сообщений типа EnergyRecordResponse
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HandleEnergyResponseTypeMessage(EnergyRecordResponse message);
    }
}
