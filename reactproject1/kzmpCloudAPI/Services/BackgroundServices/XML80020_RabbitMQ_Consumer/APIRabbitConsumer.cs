using HangfireJobsToRabbitLibrary.Models;
using kzmpCloudAPI.Interfaces;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.RabbitMQ;
using System.Text;
using static kzmpCloudAPI.Components.General.AppConsts;

namespace kzmpCloudAPI.Services.BackgroundServices.XML80020_RabbitMQ_Consumer
{
    public class APIRabbitConsumer : RabbitConsumer
    {
        IRabbitMessagesHandler _messages_handler;
        ILogger<APIRabbitConsumer> _logger;
        object _handling_lock = new object();

        public APIRabbitConsumer(IRabbitMQPersistentConnection persistent_connection, IRabbitMessagesHandler messages_handler, string def_queue_name, string def_exchange_name, ILogger<APIRabbitConsumer> logger) : base(persistent_connection, def_queue_name, def_exchange_name)
        {
            this._messages_handler = messages_handler;
            _logger = logger;
        }
        public override Task<bool> HandleBrokerMessage(byte[] message)
        {
            //Блокировка HandleBrokerMessage для других потоков
            lock (_handling_lock)
            {
                _logger.LogInformation($"New message: {Encoding.UTF8.GetString(message)}");

                //Определение типа сообщения
                var _message_str = _messages_handler.BrokerMessageToString(message);
                var _message_type = _messages_handler.GetTypeOfBrokerMessage(_message_str);

                //Сопоставление типа сообщения с типами из перечисления broker_messages_types
                //и запуск соответствующей типу функции обработки сообщения  
                switch (_message_type)
                {
                    case broker_messages_types.shedule_log_type:
                        _messages_handler.HandleSheduleLogTypeMessage(
                            message: _messages_handler.DeserializeBrokerMessage<SheduleLog>(_message_str));
                        break;
                    case broker_messages_types.power_profiles_broker_message_type:
                        _messages_handler.HandlePowerProfilesTypeMessage(
                            message: _messages_handler.DeserializeBrokerMessage<PowerProfilesBrokerMessage>(_message_str));
                        break;
                    case broker_messages_types.broker_task_message_type:
                        _messages_handler.HandleBrokerTaskTypeMessage(
                            message: _messages_handler.DeserializeBrokerMessage<BrokerTaskMessage>(_message_str));
                        break;
                    case broker_messages_types.energy_response_message_type:
                        _messages_handler.HandleEnergyResponseTypeMessage(
                            message: _messages_handler.DeserializeBrokerMessage<EnergyRecordResponse>(_message_str));
                        break;
                }
            }
            return Task.FromResult(true);
        }

    }
}
