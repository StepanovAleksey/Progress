namespace RabbitMQLibrary.RabbitMQ.Abstract
{
    /// <summary>
    /// Класс настроек для Rabbit загружаемых из json в конвеере
    /// </summary>
    public class RabbitMQOptions : IRabbitMQOptions
    {
        public const string Options = "RabbitMQConfig";
        public List<string> QueueName { get; set; } = new();
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? VirtualHost { get; set; }
        public string? HostName { get; set; }

    }
}