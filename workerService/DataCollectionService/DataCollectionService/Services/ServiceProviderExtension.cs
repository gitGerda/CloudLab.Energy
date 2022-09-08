using DataCollectionService.Components;
using DataCollectionService.Interfaces.IServices.IAppServices;
using DataCollectionService.Services.AppServices;
using DataCollectionService.Services.BackgroundServices;
using KzmpEnergyIndicationsLibrary.Actions.Connecting;
using KzmpEnergyIndicationsLibrary.Interfaces.IActions;
using RabbitMQ.Client;
using RabbitMQLibrary.Interfaces;
using RabbitMQLibrary.RabbitMQ;

namespace DataCollectionService.Services
{
    public static class ServiceProviderExtension
    {
        public static void AddRabbitMQConsumerBackHostedService(IServiceCollection _services)
        {
            _services.AddHostedService(sp =>
            {
                var _logger = sp.GetRequiredService<ILogger<RabbitMqConsumerBackService>>();
                var _consumer = sp.GetRequiredService<IRabbitConsumer>();
                var _cancellation_token_source = new CancellationTokenSource();
                var _cancellation_token = _cancellation_token_source.Token;

                return new RabbitMqConsumerBackService(_logger, _cancellation_token_source, _cancellation_token, _consumer);
            });
        }
        public static void AddIRabbitMQPersistConnSingletonService(IServiceCollection _services)
        {
            _services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["RABBITMQ_SERVER_NAME"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["RABBITMQ_USER_NAME"]))
                {
                    factory.UserName = configuration["RABBITMQ_USER_NAME"];
                };

                if (!string.IsNullOrEmpty(configuration["RABBITMQ_USER_PASS"]))
                {
                    factory.Password = configuration["RABBITMQ_USER_PASS"];
                };

                var _logger = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();
                return new RabbitMQPersistentConnection(connectionFactory: factory, logger: _logger);
            });
        }
        public static void AddIGSMConnectionSingletonService(this IServiceCollection _services)
        {
            _services.AddSingleton<IGSMConnection>(sp =>
            {
                var _conf = sp.GetRequiredService<IConfiguration>();
                var _com_port_name = _conf["DEFAULT_COM_PORT"] ?? "";
                return new Connection(portName: _com_port_name) as IGSMConnection;
            });
        }
        public static void AddIRabbitMessageGenSingletonService(this IServiceCollection _services)
        {
            _services.AddSingleton<IRabbitMessageGen>(sp =>
            {
                var _rabbit_publisher = sp.GetRequiredService<IRabbitPublisher>();
                var _logger = sp.GetRequiredService<ILogger<RabbitMessageGen>>();
                return new RabbitMessageGen(_rabbit_publisher, _logger);
            });
        }
        public static void AddIIndicationsReadingSessionSingletonService(this IServiceCollection _services)
        {
            _services.AddSingleton<IIndicationsReadingSession>(sp =>
                {
                    var _gsm_connection = sp.GetRequiredService<IGSMConnection>();
                    var _rabbit_message_gen = sp.GetRequiredService<IRabbitMessageGen>();
                    var _logger = sp.GetRequiredService<ILogger<IndicationsReadingSession>>();
                    return new IndicationsReadingSession(_gsm_connection, _rabbit_message_gen, _logger);
                });
        }
        public static void AddIIndicationsReaderSingletonService(this IServiceCollection _services)
        {
            _services.AddSingleton<IIndicationsReader>(sp =>
            {
                var _logger = sp.GetRequiredService<ILogger<IndicationsReader>>();
                var _conf = sp.GetRequiredService<IConfiguration>();
                var _gsm_connection = sp.GetRequiredService<IGSMConnection>();
                var _rabbit_message_gen = sp.GetRequiredService<IRabbitMessageGen>();
                var _indications_reading_session = sp.GetRequiredService<IIndicationsReadingSession>();

                return new IndicationsReader(_conf, _logger, _gsm_connection, _rabbit_message_gen, _indications_reading_session);
            });
        }
        public static void AddRabbitMQConsumerSingletonService(this IServiceCollection _services)
        {
            _services.AddSingleton<IRabbitConsumer>(_sp =>
            {
                var _indic_reader = _sp.GetRequiredService<IIndicationsReader>();
                var _rabbit_conn = _sp.GetRequiredService<IRabbitMQPersistentConnection>();

                var _conf = _sp.GetRequiredService<IConfiguration>();
                string _def_queue_name = _conf["DEFAULT_CONSUMER_QUEUE_NAME"] ?? "";
                string _def_exchange_name = _conf["DEFAULT_EXCHANGE_NAME"] ?? "";

                var _logger = _sp.GetRequiredService<ILogger<DataCollectRabbitConsumer>>();
                var _rabbit_messaging_ext_logger = _sp.GetRequiredService<ILogger<RabbitMessagingExtension>>();
                var _rabbit_messaging_ext = new RabbitMessagingExtension(_rabbit_messaging_ext_logger);

                return new DataCollectRabbitConsumer(indic_reader: _indic_reader, rabbit_connection: _rabbit_conn, queue_name: _def_queue_name, exchange_name: _def_exchange_name, logger: _logger, _rabbit_messaging_ext);
            });
        }
        public static void AddRabbitPublisherSingletonService(this IServiceCollection _services)
        {
            _services.AddSingleton<IRabbitPublisher>(_sp =>
            {
                var _rabbit_conn = _sp.GetRequiredService<IRabbitMQPersistentConnection>();

                var _conf = _sp.GetRequiredService<IConfiguration>();
                var _def_exchange_name = _conf["DEFAULT_EXCHANGE_NAME"] ?? "";
                var _def_queue_name = _conf["DEFAULT_PUBLISHER_QUEUE_NAME"] ?? "";

                return new RabbitPublisher(rabbit_connection: _rabbit_conn, def_exchange_name: _def_exchange_name, def_queue_name: _def_queue_name);
            });
        }
    }
}
