using kzmpCloudAPI.Database.EF_Core.Tables;
using kzmpCloudAPI.Components.General;
using kzmpCloudAPI.Services.RabbitMQService;
using RabbitMQLibrary.Interfaces;

namespace kzmpCloudAPI.Services.BackgroundServices.XML80020_RabbitMQ_Consumer
{
    public class Xml80020ConsumerService : BackgroundService
    {
        ILogger<Xml80020ConsumerService> _logger;
        IRabbitConsumer _consumer;

        public Xml80020ConsumerService(ILogger<Xml80020ConsumerService> logger, IRabbitConsumer consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
                    _consumer.persistentConnection.CreateLogRecordAsync(AppConsts.STATUS_ERROR, ex.Message);
                }
            });
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.persistentConnection.CreateLogRecordAsync(AppConsts.STATUS_INFO, "XML80020_RabbitMQ_Consumer starting...");
            await ExecuteAsync(cancellationToken);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _consumer.persistentConnection.CreateLogRecordAsync(AppConsts.STATUS_INFO, "XML80020_RabbitMQ_Consumer stopping...");
            _consumer.persistentConnection.Dispose();
            _consumer.Dispose();

        }
    }
}
