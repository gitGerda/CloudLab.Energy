using DataCollectionService.Interfaces.IServices.IAppServices;
using HangfireJobsToRabbitLibrary.Models;
using KzmpEnergyIndicationsLibrary.Models.Indications;
using Newtonsoft.Json;
using RabbitMQLibrary.Components;
using RabbitMQLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollectionService.Services.AppServices
{
    public class RabbitMessageGen : IRabbitMessageGen
    {
        private IRabbitPublisher _rabbit_publisher;
        private ILogger<RabbitMessageGen> _logger;

        public RabbitMessageGen(IRabbitPublisher rabbit_publisher, ILogger<RabbitMessageGen> logger)
        {
            _rabbit_publisher = rabbit_publisher;
            _logger = logger;
        }
        public bool PublishMessageToRabbit<T>(T message)
        {
            try
            {
                CheckConnectionAndChannel();
                var _message = GetBytesFromMessage(message);
                _rabbit_publisher.PublishMessage(_rabbit_publisher.publisher_channel, exchange_name: _rabbit_publisher.def_exchange_name, routing_key: _rabbit_publisher.def_queue_name, message: _message);
                return true;
            }
            catch
            {
                return false;
            }
        }
        internal void CheckConnectionAndChannel()
        {
            if (_rabbit_publisher.publisher_channel?.IsClosed ?? true)
            {
                _logger.LogWarning("Could not publish broker message, because channel closed or connection losed");
                while (true)
                {
                    if (_rabbit_publisher.rabbit_connection?.IsConnected ?? false)
                    {
                        _logger.LogWarning("Connection - ok; trying create channel");
                        _rabbit_publisher.publisher_channel = _rabbit_publisher.CreateChannel(exchange_name: _rabbit_publisher.def_exchange_name, queue_name: _rabbit_publisher.def_exchange_name);
                    }
                    else
                    {
                        _logger.LogWarning("Connection - false; trying reconnect");
                        if (_rabbit_publisher.rabbit_connection?.TryConnect() ?? false)
                        {
                            _rabbit_publisher.publisher_channel = _rabbit_publisher.CreateChannel(exchange_name: _rabbit_publisher.def_exchange_name, queue_name: _rabbit_publisher.def_exchange_name);
                        }
                    }
                    if (_rabbit_publisher.publisher_channel?.IsOpen ?? false)
                    {
                        _logger.LogWarning("connection - ok; channel created");
                        break;
                    }
                    _logger.LogWarning("Bad attempt. Pause on 60 0000");
                    Task.Delay(60000).Wait();
                }
            }
        }
        internal virtual byte[] GetBytesFromMessage<T>(T message)
        {
            string _serialized_message = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(_serialized_message);
        }
        public SheduleLog CreateSheduleLog(int shedule_id, string status, string description, DateTime? date = null)
        {
            return new SheduleLog()
            {
                date_time = date ?? DateTime.Now,
                shedule_id = shedule_id,
                status = status,
                description = description
            };
        }

    }
}
