using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.RabbitMQ.Abstract;
using RabbitMQLibrary.RabbitMQ.DTO;
using SignalRChatServer.Abstracts;
using System.Text;
using System.Text.Json;


namespace RabbitMQLibrary
{
    public class RabbitMQConsumerChat : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumerChat> _logger;
        private IConnection _connection;
        private IDictionary<string, IModel> _channel = new Dictionary<string, IModel>();
       // private IRepository _repo;
        public RabbitMQConsumerChat(ILogger<RabbitMQConsumerChat> logger, IRabbitMQOptions config)//, IRepository repo)
        {
           // _repo = repo;
            _logger = logger;
            //Создаём один один объект подключения на всю сессию и уничтожаем его по окончанию
            _connection = new RabbitMQConnection(config).GetRabbitConnection();
            //Создаём словарь с каналами на случай если у нас их больше чем один
            foreach (var item in config.QueueName)
            {
                try
                {
                     IModel _tempChannel = _connection.CreateModel();
                    _tempChannel.QueueDeclare(queue: item, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    _channel.Add(item, _tempChannel);
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"{DateTimeOffset.Now} Exception : {e.Message}");
                }

            }            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            foreach (var item in _channel)
            {
                var consumer = new EventingBasicConsumer(item.Value);
                consumer.Received += (ch, ea) =>
                {
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                    //Собираем полученные данные
                    ChatUserDTO? chatUser =JsonSerializer.Deserialize<ChatUserDTO>(content);
                   // _repo.AddUser(new() { GroupName = chatUser?.GroupName, Name=chatUser.Name }) ;
                    _logger.LogInformation($"{DateTimeOffset.Now} ressived ChatUserDTO [Guid: {chatUser.Id}, Status: {chatUser.Status}]");
                    item.Value.BasicAck(ea.DeliveryTag, false);
                };

                item.Value.BasicConsume(item.Key, false, consumer);
            }
           return Task.CompletedTask ;
        }

        public override void Dispose()
        {
            foreach (var item in _channel)
            item.Value.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
