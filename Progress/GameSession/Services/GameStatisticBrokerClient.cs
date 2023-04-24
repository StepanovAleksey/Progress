using GameSession.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace GameSession.Services
{
    public class GameStatisticBrokerClient
    {
        private readonly UserStatisticIntegrationOption userStstkSrvOption;

        private ConcurrentQueue<string> messages = new ConcurrentQueue<string>();

        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        };

        public GameStatisticBrokerClient(IOptions<UserStatisticIntegrationOption> userStstkSrvOption)
        {
            this.userStstkSrvOption = userStstkSrvOption.Value;
            new Timer(DropMessages, null, 0, 5 * 1000);

        }

        private ConnectionFactory CreateRabbitMqConnection()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = userStstkSrvOption.UserName;
            factory.Password = userStstkSrvOption.Password;
            factory.VirtualHost = userStstkSrvOption.VirtualHost;
            factory.HostName = userStstkSrvOption.HostName; // hostname
            return factory;
        }

        public void PushMsg<T>(T payload)
        {
            messages.Enqueue(JsonSerializer.Serialize(payload, options));
        }

        private void DropMessages(Object stateInfo)
        {
            ConnectionFactory factory = CreateRabbitMqConnection();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            while (messages.Count > 0 && messages.TryDequeue(out var message))
            {
                var messageDecode = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                                        exchange: string.Empty,
                                        routingKey: userStstkSrvOption.QueueName,
                                        basicProperties: null,
                                        body: messageDecode);
            }


        }
    }
}
