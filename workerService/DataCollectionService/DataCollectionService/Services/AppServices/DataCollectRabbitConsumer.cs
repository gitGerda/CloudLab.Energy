using DataCollectionService.Components;
using DataCollectionService.Interfaces.IComponents;
using DataCollectionService.Interfaces.IServices.IAppServices;
using HangfireJobsToRabbitLibrary.Models;
using KzmpEnergyIndicationsLibrary.Variables;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.Components;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Services.AppServices
{
    public class DataCollectRabbitConsumer : RabbitConsumer
    {
        IIndicationsReader _indic_reader
        {
            get; set;
        }
        ILogger<DataCollectRabbitConsumer> _logger;
        IRabbitMessagingExtension _rabbit_messaging_ext;
        public DataCollectRabbitConsumer(IIndicationsReader indic_reader, IRabbitMQPersistentConnection rabbit_connection, string queue_name, string exchange_name, ILogger<DataCollectRabbitConsumer> logger, IRabbitMessagingExtension rabbit_messaging_ext) : base(persistent_connection: rabbit_connection, def_exchange_name: exchange_name, def_queue_name: queue_name)
        {
            _indic_reader = indic_reader;
            _logger = logger;
            _rabbit_messaging_ext = rabbit_messaging_ext;
        }
        object _handling_lock = new object();
        bool _handling_lock_was_taken = false;

        public override async Task ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            //Если доступ к ConsumerReceived заблокирован 
            if (_handling_lock_was_taken == true)
            {
                _logger.LogCritical($"An attempt to process a message from a broker with a blocked Consumer Received. Broker's Message: {_indic_reader.BrokerMessageToString(eventArgs.Body.Span.ToArray())}");
                return;
            }

            //Блокируем доступ к ConsumerReceived другим потокам
            lock (_handling_lock)
            {
                _logger.LogWarning("Consumer Received function blocked");
                _handling_lock_was_taken = true;
                try
                {
                    var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
                    if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                    {
                        throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                    }
                    //Отвечаем сразу, что сообщение успешно получено и обработано
                    //После приёма сообщения закрываем канал и обрабатываем сообщение
                    _rabbit_messaging_ext.BasicAckOkAndCloseConnection(this, eventArgs);

                    var _result = HandleBrokerMessage(eventArgs.Body.Span.ToArray()).Result;
                    _logger.LogInformation("Broker message successfully handled");
                }
                catch (Exception ex)
                {
                    persistentConnection?.CreateLogRecordAsync(LibConsts.STATUS_ERROR, ex.Message).Wait();
                    _logger.LogCritical(ex.Message);
                }
                finally
                {
                    //Освобождаем блокировку ConsumerReceived
                    _logger.LogWarning("Releasing the Consumer Received function lock");
                    _handling_lock_was_taken = false;
                    //После того, как сообщение обработано начинаем снова принимать сообщения
                    _rabbit_messaging_ext.CreateChannelAndStartConsuming(this);
                }
            }
        }
        public override async Task<bool> HandleBrokerMessage(byte[] message)
        {
            var _message_obj = _indic_reader.BrokerMessageToString(message);
            var _message_type = _indic_reader.GetTypeOfBrokerMessage(message: _message_obj ?? "");
            bool result = false;

            switch (_message_type)
            {
                case AppConsts.LIST_BROKER_TASK_MESSAGE_TYPE:
                    result = await _indic_reader.GetIndications(_indic_reader.DeserializeBrokerMessage<List<BrokerTaskMessage>>(_message_obj));
                    break;
                case AppConsts.PORT_CONFIGURATION_TYPE:
                    result = _indic_reader.ChangeComPortName(_indic_reader.DeserializeBrokerMessage<PortConfiguration>(_message_obj).PortName);
                    break;
                default:
                    throw new Exception("Invalid broker message type");
            }

            return result;
        }

    }
}
