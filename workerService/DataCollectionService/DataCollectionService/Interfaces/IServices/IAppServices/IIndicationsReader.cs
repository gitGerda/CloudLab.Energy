using HangfireJobsToRabbitLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Interfaces.IServices.IAppServices
{
    public interface IIndicationsReader
    {
        /// <summary>
        /// Функция для изменения названия COM-порта
        /// </summary>
        /// <param name="com_port"></param>
        /// <returns></returns>
        public bool ChangeComPortName(string com_port);
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
        public int? GetTypeOfBrokerMessage(string message);
        /// <summary>
        /// Чтение показаний со счётчиков и отправка результатов в брокер сообщений
        /// </summary>
        /// <param name="broker_message"></param>
        /// <returns></returns>
        public Task<bool> GetIndications(List<BrokerTaskMessage> broker_message);
        /// <summary>
        /// Фильтрация broker_message по полю sim_number и создание очереди листов
        /// с одинаковыми sim_number
        /// </summary>
        /// <param name="broker_message"></param>
        /// <returns></returns>
        public Queue<List<BrokerTaskMessage>> FilterBrokerTaskMessage(List<BrokerTaskMessage> broker_message);
        /// <summary>
        /// Старт сеанса чтения показаний
        /// </summary>
        /// <param name="serial_port"></param>
        /// <param name="reading_task"></param>
        /// <returns></returns>
        public Task<bool> StartIndicationsReadingSession(ref SerialPort serial_port, ref List<BrokerTaskMessage> reading_task);
    }
}
