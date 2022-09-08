using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLibrary.Interfaces
{
    public interface IRabbitPublisher
    {

        /// <summary>
        /// Канал публикации по умолчанию   
        /// </summary>
        public IModel? publisher_channel
        {
            get; set;
        }
        /// <summary>
        /// Создание канала с брокером сообщений
        /// </summary>
        /// <param name="exchange_name"></param>
        /// <param name="queue_name"></param>
        /// <returns></returns>
        public IModel CreateChannel(string exchange_name, string queue_name);
        /// <summary>
        /// Открытие канала, отправка сообщения, закрытие канала
        /// </summary>
        /// <param name="queue_name"></param>
        /// <param name="exchange_name"></param>
        /// <param name="routing_key"></param>
        /// <param name="message"></param>
        public void PublishOneMessageAndCloseChannel(string queue_name, string exchange_name, string routing_key, byte[] message);

        /// <summary>
        /// Соединение с брокером сообщений
        /// </summary>
        public IRabbitMQPersistentConnection? rabbit_connection
        {
            get; set;
        }
        /// <summary>
        /// Наименование Exchange по умолчанию
        /// </summary>
        public string def_exchange_name
        {
            get; set;
        }
        /// <summary>
        /// Наименование очереди по умолчанию
        /// </summary>
        public string def_queue_name
        {
            get; set;
        }
        /// <summary>
        /// Публикация сообщения
        /// </summary>
        ///<param name="channel"></param>
        /// <param name="exchange_name"></param>
        /// <param name="routing_key"></param>
        /// <param name="message"></param>
        public void PublishMessage(IModel? channel, string exchange_name, string routing_key, byte[] message);
        /// <summary>
        /// Отправка сообщения в брокер
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="exchange_name"></param>
        /// <param name="routing_key"></param>
        public void SendMessage(IModel? channel, byte[] message, string exchange_name, string routing_key);
        /// <summary>
        /// Объявление exchange брокера сообщений
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchange_name"></param>
        public void DeclareExchange(IModel? channel, string exchange_name);
        /// <summary>
        /// Объявление и привязка очереди сообщений
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queue_name"></param>
        /// <param name="exchange_name"></param>
        public void DeclareAndBindQueue(IModel? channel, string queue_name, string exchange_name);
    }
}
