using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.Components;
using RabbitMQLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLibrary.RabbitMQ
{
    public class RabbitConsumer : IRabbitConsumer
    {
        private IRabbitMQPersistentConnection? _persistentConnection;
        private IModel? _consumerChannel;
        public IRabbitMQPersistentConnection? persistentConnection
        {
            get => _persistentConnection;
            set => _persistentConnection = value;
        }
        public IModel? consumerChannel
        {
            get => _consumerChannel;
            set => _consumerChannel = value;
        }
        protected string _def_queue_name;
        protected string _def_exchange_name;

        public RabbitConsumer(IRabbitMQPersistentConnection persistent_connection, string def_queue_name, string def_exchange_name)
        {
            this.persistentConnection = persistent_connection ?? throw new ArgumentNullException(nameof(persistent_connection));
            _def_queue_name = def_queue_name;
            _def_exchange_name = def_exchange_name;
        }
        public virtual async Task ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                var _result = await HandleBrokerMessage(eventArgs.Body.Span.ToArray());

                if (_result == true)
                    _consumerChannel?.BasicAck(eventArgs.DeliveryTag, false);

            }
            catch (Exception ex)
            {
                await persistentConnection.CreateLogRecordAsync(LibConsts.STATUS_ERROR, ex.Message);
                _consumerChannel?.BasicReject(eventArgs.DeliveryTag, true);
            }
        }

        public IModel CreateDefaultConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                 persistentConnection.TryConnect();
            }
            persistentConnection.CreateLogRecordAsync(LibConsts.STATUS_INFO, "Creating RabbitMQ consumer channel");

            var channel = persistentConnection.CreateModel();
            channel.BasicQos(0, 1, false);

            channel.ExchangeDeclare(exchange: _def_exchange_name,
                type: ExchangeType.Direct,
                durable: true);

            channel.QueueDeclare(queue: _def_queue_name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: _def_queue_name,
                exchange: _def_exchange_name,
                routingKey: _def_queue_name,
                arguments: null);

            channel.CallbackException += (sender, e) =>
            {
                persistentConnection.CreateLogRecordAsync(LibConsts.STATUS_WARNING, "Recreating RabbitMQ consumer channel");

                _consumerChannel?.Dispose();
                _consumerChannel = CreateDefaultConsumerChannel();
                StartDefaultConsume();
            };

            return channel;
        }

        public virtual Task<bool> HandleBrokerMessage(byte[] message)
        {
            return Task.FromResult(true);
        }


        public void StartDefaultConsume()
        {
            persistentConnection.CreateLogRecordAsync(LibConsts.STATUS_INFO, "Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.Received += ConsumerReceived;
                

                _consumerChannel.BasicConsume(queue: _def_queue_name,
                    autoAck: false,
                    consumerTag:"",
                    consumer: consumer);
            }
            else
            {
                persistentConnection.CreateLogRecordAsync(LibConsts.STATUS_ERROR, "StartBasicConsume can't call on _consumerChannel == null");
            }
        }
        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
        }

    }
}
