namespace RabbitMQLibrary.RabbitMQ.Abstract
{
    public interface IRabbitMQPubliserService
    {
        void SendMessage<T>(T obj) where T:class ;
        void SendMessage(string? message);
    }
}
