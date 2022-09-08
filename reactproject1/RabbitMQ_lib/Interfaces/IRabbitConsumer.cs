using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLibrary.Interfaces
{
    public interface IRabbitConsumer : IDisposable
    {
        /// <summary>
        /// Соединение с брокером сообщений
        /// </summary>
        public IRabbitMQPersistentConnection? persistentConnection
        {
            get; set;
        }
        /// <summary>
        /// Канал потребителя сообщений брокера
        /// </summary>
        public IModel? consumerChannel
        {
            get; set;
        }
        /// <summary>
        /// Создание канала брокера сообщений
        /// </summary>
        /// <returns></returns>
        public IModel CreateDefaultConsumerChannel();
        /// <summary>
        /// Функция прослушивания очереди брокера сообщений 
        /// </summary>
        public void StartDefaultConsume();
        /// <summary>
        /// Функция принимающая входящие сообщения 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public Task ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs);
        /// <summary>
        /// Обработка полученного сообщения
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<bool> HandleBrokerMessage(byte[] message);
    }
}
