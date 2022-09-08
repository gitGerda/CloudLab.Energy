using RabbitMQ.Client;
using RabbitMQLibrary.Components;
using RabbitMQLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLibrary.RabbitMQ
{
    public class RabbitPublisher : IRabbitPublisher
    {

        private IModel? _publisher_channel;
        public IModel? publisher_channel
        {
            get => _publisher_channel;
            set => _publisher_channel = value;
        }
        private IRabbitMQPersistentConnection? _rabbit_connection;
        public IRabbitMQPersistentConnection? rabbit_connection
        {
            get => _rabbit_connection;
            set => _rabbit_connection = value;
        }
        private string _def_exchange_name;
        private string _def_queue_name;
        public string def_exchange_name
        {
            get => _def_exchange_name;
            set => _def_exchange_name = value;
        }
        public string def_queue_name
        {
            get => _def_queue_name;
            set => _def_queue_name = value;
        }

        public RabbitPublisher(IRabbitMQPersistentConnection rabbit_connection, string def_exchange_name, string def_queue_name)
        {
            this.rabbit_connection = rabbit_connection;
            this.def_exchange_name = def_exchange_name;
            this.def_queue_name = def_queue_name;
            this.publisher_channel = CreateChannel(exchange_name: def_exchange_name, queue_name: def_exchange_name);
        }

        public virtual void DeclareAndBindQueue(IModel? channel, string queue_name, string exchange_name)
        {
            channel?.QueueDeclare(queue: queue_name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel?.QueueBind(queue: queue_name,
                exchange: exchange_name,
                routingKey: queue_name,
                arguments: null);
        }

        public virtual void DeclareExchange(IModel? channel, string exchange_name)
        {
            channel?.ExchangeDeclare(exchange: exchange_name,
                     type: ExchangeType.Direct,
                     durable: true,
                     autoDelete: false,
                     arguments: null);
        }

        public void PublishOneMessageAndCloseChannel(string queue_name, string exchange_name, string routing_key, byte[] message)
        {
            if (!_rabbit_connection.IsConnected)
                _rabbit_connection.TryConnect();

            var _channel = _rabbit_connection.CreateModel();
            DeclareExchange(channel: _channel, exchange_name: exchange_name);
            DeclareAndBindQueue(channel: _channel, queue_name: queue_name, exchange_name: exchange_name);
            SendMessage(channel: _channel, message: message, exchange_name: exchange_name, routing_key: routing_key);

            if (_channel.IsOpen)
                _channel.Close();
        }
        public void PublishMessage(IModel? channel, string exchange_name, string routing_key, byte[] message)
        {
            if (channel?.IsOpen ?? false)
            {
                SendMessage(channel: channel, message: message, exchange_name: exchange_name, routing_key: routing_key);
            }
            else
            {
                rabbit_connection?.CreateLogRecordAsync(LibConsts.STATUS_WARNING, "PublishMessage can't call on _publisher_channel == null");
            }

        }
        public IModel? CreateChannel(string exchange_name, string queue_name)
        {
            if (rabbit_connection == null)
            {
                return null;
            }
            if (!rabbit_connection?.IsConnected ?? false)
            {
                rabbit_connection?.TryConnect();
            }
            rabbit_connection?.CreateLogRecordAsync(LibConsts.STATUS_INFO, "Creating RabbitMQ publisher channel");

            var _channel = rabbit_connection?.CreateModel();

            DeclareExchange(channel: _channel, exchange_name: exchange_name);
            DeclareAndBindQueue(channel: _channel, queue_name: queue_name, exchange_name: exchange_name);

            _channel.CallbackException += (sender, e) =>
            {
                rabbit_connection?.CreateLogRecordAsync(LibConsts.STATUS_WARNING, "Recreating RabbitMQ publisher channel");

                publisher_channel?.Dispose();
                publisher_channel = CreateChannel(exchange_name, queue_name);
            };
            return _channel;
        }

        public virtual void SendMessage(IModel? channel, byte[] message, string exchange_name, string routing_key)
        {
            if (channel == null)
                return;

            IBasicProperties? _basic_props = channel?.CreateBasicProperties();
            _basic_props.Persistent = true;

            channel?.BasicPublish(exchange: exchange_name,
                routingKey: routing_key,
                basicProperties: _basic_props,
                body: message);
        }
    }
}
