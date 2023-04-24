using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQLibrary.RabbitMQ;
using System.Threading.Channels;
using RabbitMQLibrary.RabbitMQ.Abstract;

namespace RabbitMQLibrary
{
    public class RabbitMQPubliserService: IRabbitMQPubliserService, IDisposable
    {
        private IConnection _connection;
        private IRabbitMQOptions _options;
        public RabbitMQPubliserService(IRabbitMQOptions options)
        {
            _options = options;
            _connection = new RabbitMQConnection(options).GetRabbitConnection();
        }
        public void SendMessage<T>(T obj) where T:class
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string? message)
        {
            foreach (var queueName in _options.QueueName)
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                   durable: false,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                   routingKey: queueName,
                                   basicProperties: null,
                                   body: body);
                }
            }
        }
        public void Dispose()
        {
            _connection.Close();
        }
    }
}
