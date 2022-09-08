using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCollectionService.Interfaces.IServices.IBackgroundServices;
using RabbitMQLibrary.Components;
using RabbitMQLibrary.Interfaces;

namespace DataCollectionService.Services.BackgroundServices
{
    public class RabbitMqConsumerBackService : IRabbitMqConsumerService
    {
        CancellationTokenSource _cancelation_source;
        CancellationToken _cancelation_token;
        ILogger<RabbitMqConsumerBackService> _logger;
        IRabbitConsumer _consumer;
        public RabbitMqConsumerBackService(ILogger<RabbitMqConsumerBackService> logger, CancellationTokenSource cancellation_token_source, CancellationToken cancellation_token, IRabbitConsumer rabbit_consumer)
        {
            _cancelation_source = cancellation_token_source;
            _cancelation_token = cancellation_token;
            _logger = logger;
            _consumer = rabbit_consumer;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} -> RabbitMQ Consumer Service starting...");
            await ExecuteAsync(_cancelation_token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning($"{DateTime.Now} -> Attempt to stop RabbitMQ Consumer Service...");
            CloseService(_cancelation_source);
            return Task.CompletedTask;
        }

        public virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogWarning("RabbitMQ Service stopped."));

            _logger.LogInformation("Background Service started");
            await Task.Run(async () =>
            {
                try
                {
                    for (int i = 0; i < 15; i++)
                    {
                        if (!_consumer.persistentConnection.TryConnect())
                        {
                            await Task.Delay(millisecondsDelay: 3000);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (_consumer.persistentConnection.IsConnected)
                    {
                        _consumer.consumerChannel = _consumer.CreateDefaultConsumerChannel();
                        _consumer.StartDefaultConsume();
                    }
                    else
                    {
                        await StopAsync(stoppingToken);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _consumer.persistentConnection.CreateLogRecordAsync(LibConsts.STATUS_ERROR, ex.Message);
                }
            });
        }
        public virtual void CloseService(CancellationTokenSource _source)
        {
            _source.Cancel();
        }
    }
}
