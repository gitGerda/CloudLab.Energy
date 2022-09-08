using Hangfire;
using Hangfire.MySql;
using kzmpCloudAPI.Components.HangfireSheduler;
using kzmpCloudAPI.Components.IndicationsReading;
using kzmpCloudAPI.Services.RabbitMQService;
using HangfireJobsToRabbitLibrary.Jobs.IndicationsReading;
using kzmpCloudAPI.Components.HangfireSheduler.UpdateShedulePart;
using kzmpCloudAPI.Database.EF_Core;
using RabbitMQ.Client;
using RabbitMQLibrary.RabbitMQ;
using RabbitMQLibrary.Interfaces;
using kzmpCloudAPI.Services.BackgroundServices.XML80020_RabbitMQ_Consumer;
using kzmpCloudAPI.Interfaces;

namespace kzmpCloudAPI.Services
{
    public static class ServiceProviderExtension
    {
        public static void AddFilesCompressingScopedService(this IServiceCollection services)
        {
            services.AddScoped<IFilesCompressingService>(impl =>
            {
                var _logger = impl.GetRequiredService<ILogger<FilesCompressingService>>();
                return new FilesCompressingService(_logger);
            });
        }
        public static void AddReportXml80020TransientService(this IServiceCollection services)
        {
            services.AddTransient<ReportXML80020Service>();
        }

        public static void AddHangfireShedulerCustomService(this IServiceCollection services)
        {
            services.AddScoped<IHangfireSheduler<SheduleCreateUpdateModel>>(
                impl =>
                {
                    IConfiguration _conf = impl.GetRequiredService<IConfiguration>();
                    var _database = impl.GetRequiredService<kzmp_energyContext>();
                    var _http_client = impl.GetRequiredService<HttpClient>();

                    string _hangfire_storage_conn_str = _conf["HANGFIRE_STORAGE_CONN_STR"];
                    string _web_server_address = _conf["WEB_SERVER_ADDRESS"];

                    JobStorage.Current = new MySqlStorage(_hangfire_storage_conn_str, new MySqlStorageOptions());

                    var _hangfire_sheduler = new HangfireShedulerService(
                        update_shedule: new UpdateShedule(_database),
                        job: new IndicationsReadingJob(new HangfireRabbitJobs(), _http_client),
                        conf: _conf,
                        database: _database);

                    return _hangfire_sheduler;
                }
            );
        }
        public static void AddRabbitMQPersistentConnectionSingletonService(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
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

                return new DefaultRabbitMQPersistentConnection(connection_factory: factory, logger: _logger, configuration: configuration);
            });
        }

        public static void AddIndicationHandlerSingletonService(this IServiceCollection _services)
        {
            _services.AddSingleton<IRabbitMessagesHandler>(sp =>
            {
                var _conf = sp.GetRequiredService<IConfiguration>();
                return new RabbitMessagesHandler(configuration: _conf);
            });
        }
        public static void AddRabbitMQConsumerSingletonService(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitConsumer>(sp =>
            {
                var persistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var configuration = sp.GetRequiredService<IConfiguration>();
                var _def_queue = configuration["DEFAULT_QUEUE_NAME"] ?? "";
                var _def_exchange = configuration["DEFAULT_EXCHANGE_NAME"] ?? "";
                var _indic_handler = sp.GetRequiredService<IRabbitMessagesHandler>();
                var _logger = sp.GetRequiredService<ILogger<APIRabbitConsumer>>();

                return new APIRabbitConsumer(persistent_connection: persistentConnection,
                    def_queue_name: _def_queue,
                    def_exchange_name: _def_exchange,
                    messages_handler: _indic_handler,
                    logger: _logger);
            });
        }
        public static void AddRabbitMQPublisherConsumerSingletonService(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitPublisher>(sp =>
            {
                var persistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var configuration = sp.GetRequiredService<IConfiguration>();
                var _def_exchange = configuration["DEFAULT_EXCHANGE_NAME"] ?? "";
                var _def_queue = configuration["DEFAULT_QUEUE_NAME"] ?? "";

                return new RabbitPublisher(rabbit_connection: persistentConnection, def_exchange_name: _def_exchange, def_queue_name: _def_queue);
            });
        }
    }
}
