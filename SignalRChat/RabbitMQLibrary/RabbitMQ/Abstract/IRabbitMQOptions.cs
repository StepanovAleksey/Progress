namespace RabbitMQLibrary.RabbitMQ.Abstract
{
    /// <summary>
    /// Стандартные параметры подключения RabbitMQ
    /// </summary>
    public interface IRabbitMQOptions
    {
        public List<string> QueueName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? VirtualHost { get; set; }
        public string? HostName { get; set; }
    }
}
