using kzmpCloudAPI.Components.General;
using kzmpCloudAPI.Database.EF_Core;
using kzmpCloudAPI.Database.EF_Core.Tables;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.RabbitMQ;

namespace kzmpCloudAPI.Services.RabbitMQService
{
    public class DefaultRabbitMQPersistentConnection : RabbitMQPersistentConnection
    {
        private kzmp_energyContext _database;
        private readonly string databaseConnectionString;
        public DefaultRabbitMQPersistentConnection(IConnectionFactory connection_factory, ILogger<RabbitMQPersistentConnection> logger, IConfiguration configuration) : base(connectionFactory: connection_factory, logger: logger)
        {
            databaseConnectionString = configuration.GetConnectionString("DefaultConnection");
            _database = new kzmp_energyContext(new DbContextOptionsBuilder<kzmp_energyContext>().UseMySql(databaseConnectionString, ServerVersion.Parse("8.0.29-mysql")).Options);
        }
        public override async Task CreateLogRecordAsync(string status, string message)
        {
            var log = new RabbitMQLog()
            {
                Date = DateTime.Now,
                Status = status,
                Message = message
            };
            _database.RabbitMQLogs.Add(log);
            await _database.SaveChangesAsync();
        }
        public override bool TryConnect()
        {
            lock (sync_root)
            {
                CreateLogRecordAsync(AppConsts.STATUS_INFO, "RabbitMQ Client is trying to connect");
                try
                {
                    _connection = _connectionFactory.CreateConnection();
                }
                catch (Exception ex)
                {
                    CreateLogRecordAsync(AppConsts.STATUS_ERROR, ex.Message);
                }

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    _connection.CallbackException += OnCallbackException;

                    CreateLogRecordAsync(AppConsts.STATUS_INFO, $"RabbitMQ Client acquired a persistent connection to '{_connection.Endpoint.HostName}' and is subscribed to failure events");

                    return true;
                }
                else
                {
                    CreateLogRecordAsync(AppConsts.STATUS_ERROR, "FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }
        public override void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            base.OnConnectionBlocked(sender, e);
            CreateLogRecordAsync(AppConsts.STATUS_WARNING, "A RabbitMQ connection is shutdown. Trying to re-connect...");
        }
        public override void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            base.OnCallbackException(sender, e);
            CreateLogRecordAsync(AppConsts.STATUS_WARNING, "A RabbitMQ connection throw exception. Trying to re-connect...");
        }
        public override void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            base.OnConnectionShutdown(sender, reason);
            CreateLogRecordAsync(AppConsts.STATUS_WARNING, "A RabbitMQ connection is on shutdown. Trying to re-connect...");
        }
    }
}
