using RabbitMQLibrary;
using RabbitMQLibrary.RabbitMQ.Abstract;
using RabbitMQLibrary.RabbitMQ.DTO;
using System.Text.Json;

namespace PublisherRabbitTester
{
    internal class Program
    {
        static void Main(string[] args)        {
           
            IRabbitMQOptions options = new RabbitMQOptions() {
                UserName = "iyglhuaj",
                Password = "AtkBM4d1ytSJbIIMVtj6uVjdoBN4tz-O",
                VirtualHost = "iyglhuaj",
                HostName = "sparrow.rmq.cloudamqp.com",
                QueueName = { "GameService", "Test", "UserStatistic" }
            }; 
            IRabbitMQPubliserService mqService = new RabbitMQPubliserService(options);
            
            do
            {
                Console.WriteLine($"Нажмите Enter для отправки тестовых данных");
                var text = Console.ReadLine();
                List<ChatUserDTO> users = new () {
                    new(){  Id = Guid.NewGuid(), Status= UserStatus.On, Name="Nick"},
                    new(){  Id = Guid.NewGuid(), Status= UserStatus.On, Name="Bill"},
                    new(){  Id = Guid.NewGuid(), Status= UserStatus.On, Name="Jade"},
                    new(){  Id = Guid.NewGuid(), Status= UserStatus.On, Name="Nancy" },
                };
                foreach (var user in users)
                {   
                    text = JsonSerializer.Serialize(user);
                    mqService.SendMessage(text);
                }
            }
            while (true);

            Console.ReadLine();
        }
    }
}