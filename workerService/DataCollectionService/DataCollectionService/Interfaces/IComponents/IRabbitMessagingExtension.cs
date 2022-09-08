using RabbitMQ.Client.Events;
using RabbitMQLibrary.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Interfaces.IComponents
{
    public interface IRabbitMessagingExtension
    {
        /// <summary>
        /// Подверждение получения сообщения и закрытие соединения с брокером сообщений
        /// </summary>
        /// <param name="_rabbit_consumer"></param>
        /// <param name="eventArgs"></param>
        void BasicAckOkAndCloseConnection(RabbitConsumer _rabbit_consumer, BasicDeliverEventArgs eventArgs);
        /// <summary>
        /// Создание канала и старт consuming сообщений
        /// </summary>
        /// <param name="_rabbit_consumer"></param>
        void CreateChannelAndStartConsuming(RabbitConsumer _rabbit_consumer);
    }
}
