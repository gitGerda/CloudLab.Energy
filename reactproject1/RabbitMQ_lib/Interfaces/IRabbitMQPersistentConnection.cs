using RabbitMQ.Client;

namespace RabbitMQLibrary.Interfaces
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected
        {
            get;
        }
        bool TryConnect();
        IModel CreateModel();
        Task CreateLogRecordAsync(string status, string message);
    }
}
