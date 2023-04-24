using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLibrary.RabbitMQ.Abstract;
using RabbitMQLibrary.RabbitMQ.DTO;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using View.Models.In;
using View.Models.Out;


namespace RabbitMQLibrary
{
    //Класс на основе BackgroundService, добавляем его в конвеер сервиса для подписки и фонового прослушивания очереди RabbitMQ
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private IConnection _connection;
        private IDictionary<string, IModel> _channel = new Dictionary<string, IModel>();

        //Не забыть в кинструктор передать рерозиторий или какой либо другой объект для загрузки в дальнейшем получаемых данных
        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, IRabbitMQOptions config)
        {
            _logger = logger;
            //Создаём один один объект подключения на всю сессию и уничтожаем его по окончанию
            _connection = new RabbitMQConnection(config).GetRabbitConnection();
            //Создаём словарь с каналами на случай если у нас их больше чем один
            foreach (var item in config.QueueName)
            {
                IModel _tempChannel;
                _tempChannel = _connection.CreateModel();
                _tempChannel.QueueDeclare(queue: item, durable: false, exclusive: false, autoDelete: false, arguments: null);
                _channel.Add(item, _tempChannel);
            }            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string result = "";
            stoppingToken.ThrowIfCancellationRequested();
            foreach (var chnl in _channel)
            {
                var consumerR = new EventingBasicConsumer(chnl.Value);
                consumerR.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                };
                var t = chnl.Value.BasicConsume(chnl.Key,
                                     autoAck: true,
                                     consumer: consumerR);
                // TODO: сериализовать сообщение из очериди и вызвать - CreateUserStatistic из UserStatistic.API
            }
            return Task.CompletedTask ;
        }

        /// <summary>
        /// Освобождаем ресурсы
        /// </summary>
        public override void Dispose()
        {
            foreach (var item in _channel)
            item.Value.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
