using DataCollectionService.Interfaces.IComponents;
using KzmpEnergyIndicationsLibrary.Variables;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Components
{
    public class RabbitMessagingExtension : IRabbitMessagingExtension
    {
        ILogger<RabbitMessagingExtension> _logger;
        public RabbitMessagingExtension(ILogger<RabbitMessagingExtension> logger)
        {
            _logger = logger;
        }

        public virtual void BasicAckOkAndCloseConnection(RabbitConsumer _rabbit_consumer, BasicDeliverEventArgs eventArgs)
        {
            //Отвечаем сразу, что сообщение успешно получено и обработано
            _logger.LogWarning("Basic ack - OK");
            _rabbit_consumer.consumerChannel?.BasicAck(eventArgs.DeliveryTag, false);
            //После приёма сообщения закрываем канал и обрабатываем сообщение
            _rabbit_consumer.consumerChannel?.Close();
            _logger.LogWarning("Consumer channel closed");
            _rabbit_consumer.consumerChannel?.Dispose();
            _logger.LogWarning("Connection closing...");
            _rabbit_consumer.persistentConnection?.Dispose();
            _logger.LogWarning("Connection closed");
        }

        public virtual void CreateChannelAndStartConsuming(RabbitConsumer _rabbit_consumer)
        {
            _logger.LogInformation("Connecting to broker, creating Rabbit MQ channel and consuming starting");
            while (true)
            {
                try
                {
                    _logger.LogWarning("Create rabbit connection");
                    if (!_rabbit_consumer.persistentConnection?.TryConnect() ?? false)
                    {
                        throw new Exception("Could not connect to broker");
                    }
                    _rabbit_consumer.consumerChannel = _rabbit_consumer.CreateDefaultConsumerChannel();
                    _logger.LogWarning("Consumer channel created");
                    _logger.LogWarning($"Timeout before consuming start [{CommonVariables.REPEAT_CONNECTION_ATTEMPTS_TIMEOUT_MILISEC}]");
                    Task.Delay(CommonVariables.REPEAT_CONNECTION_ATTEMPTS_TIMEOUT_MILISEC).Wait();
                    _rabbit_consumer.StartDefaultConsume();
                    _logger.LogWarning("Consuming started");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("Channel creating or consuming starting error");
                    _logger.LogCritical(ex.Message);
                    _logger.LogWarning($"Timeout before repeat attempt to create channel [{CommonVariables.REPEAT_CONNECTION_ATTEMPTS_TIMEOUT_MILISEC}]");

                    _logger.LogWarning("Trying to close consumer channel");
                    if (_rabbit_consumer.consumerChannel?.IsOpen ?? false)
                    {
                        _logger.LogWarning("Consumer channel closing");
                        _rabbit_consumer.consumerChannel?.Close();
                        _logger.LogWarning("Consumer channel closed");
                        _rabbit_consumer.consumerChannel?.Dispose();
                    }
                    else
                    {
                        _logger.LogWarning("Consumer channel not open");
                    }

                    _logger.LogWarning($"Pause [{CommonVariables.REPEAT_CONNECTION_ATTEMPTS_TIMEOUT_MILISEC}]");
                    Task.Delay(CommonVariables.REPEAT_CONNECTION_ATTEMPTS_TIMEOUT_MILISEC).Wait();
                    continue;
                }
            }
        }
    }
}
