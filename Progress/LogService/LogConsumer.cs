using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Serilog;
using System.Text;

namespace LogService {
    public class LogConsumer {
        private IConfiguration _config;

        public LogConsumer() {
            _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        }
        public void RunRabbitMQLogConcumer() {
            var factory = new ConnectionFactory() { HostName = _config.GetSection("rabbitMQ")["hostname"] };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                exchange: "direct_logs",
                routingKey: "logs");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) => {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Log.Warning(message);
                };

                channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);
                Console.WriteLine("LogConsumer has started");
                Console.ReadLine();
            }
        }
        public void SetUpLogger() {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.PostgreSQL(
                    connectionString: _config.GetSection("loggerConnection").Value,
                    tableName: "logs",
                    needAutoCreateTable: true,
                    respectCase: true,
                    useCopy: false
                )
                .CreateLogger();
        }
    }
}
