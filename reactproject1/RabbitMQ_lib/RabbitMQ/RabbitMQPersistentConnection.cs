using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.Components;
using RabbitMQLibrary.Interfaces;

namespace RabbitMQLibrary.RabbitMQ
{
    public class RabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private ILogger<RabbitMQPersistentConnection> _logger;
        protected readonly IConnectionFactory _connectionFactory;
        protected IConnection? _connection;
        bool _disposed;
        protected object sync_root = new object();

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<RabbitMQPersistentConnection> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger;
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            try
            {
                if (IsConnected)
                {
                    _connection.ConnectionShutdown -= OnConnectionShutdown;
                    _connection.ConnectionBlocked -= OnConnectionBlocked;
                    _connection.CallbackException -= OnCallbackException;
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.Message);
            }
        }
        public virtual bool TryConnect()
        {
            lock (sync_root)
            {
                try
                {
                    _connection = _connectionFactory.CreateConnection();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    _connection.CallbackException += OnCallbackException;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed)
                return;
            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }
        public virtual void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed)
                return;
            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        }
        public virtual void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed)
                return;
            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }

        public virtual Task CreateLogRecordAsync(string status, string message)
        {
            switch (status)
            {
                case LibConsts.STATUS_INFO:
                    _logger.LogInformation(message);
                    break;
                case LibConsts.STATUS_WARNING:
                    _logger.LogWarning(message);
                    break;
                case LibConsts.STATUS_ERROR:
                    _logger.LogCritical(message);
                    break;

                default:
                    _logger.LogCritical(message);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
