using RabbitMQ.Client;
using System.Text;

namespace WebApplication2_client.rabbit
{
    public class rabbitEmitter
    {
        public void EmitMessage(string message)
        {
            string _exchangeName = "cloudlab_energy_indications_ex";
            string _routingKey = "cloudlab_energy_indications_queue";

            var connectionFactory = new ConnectionFactory()
            {
                HostName = "85.193.83.154",
                UserName = "MeasuringPointUser",
                Password = "Otohof96"
            };
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: _exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null);

            IBasicProperties _basicProps = channel.CreateBasicProperties();
            _basicProps.Persistent = true;

            channel.BasicPublish(exchange: "cloudlab_energy_indications_ex",
                routingKey: "cloudlab_energy_indications_queue",
                basicProperties: _basicProps,
                body: Encoding.UTF8.GetBytes(message));
        }
        public void Write(string mes)
        {
            string f = "dsdsds";
        }

    }
}
