using RabbitMQ.Client;

namespace RabbitMQLibrary.RabbitMQ.Abstract
{
    public class RabbitMQConnection
    {
        /// <summary>
        /// Поле хранит параметры подключения полученные из json файла сервиса
        /// </summary>
        private readonly IRabbitMQOptions _config;
        public RabbitMQConnection(IRabbitMQOptions config)
        {
            _config = config;
        }
        /// <summary>
        /// Создаёт подключение к брокеру sparrow.rmq.cloudamqp.com срок жизни учётки до 1.04.2023
        /// </summary>
        /// <returns>Возвращает объект соединения или null если приподключении возникла ошибка</returns>
        public IConnection GetRabbitConnection()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = _config.UserName,
                Password = _config.Password,
                VirtualHost = _config.VirtualHost,
                HostName = _config.HostName
            };
            try
            {
                IConnection conn = factory.CreateConnection();
                return conn;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
