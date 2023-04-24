using SignalRChatServer.Abstracts;
using SignalRChat.Hubs;
using RabbitMQLibrary;
using RabbitMQLibrary.RabbitMQ.Abstract;

namespace SignalRChatServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            //Подключаем SignalR
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IRepository, Repository>();
            //Созлаем обьект опций подключения для RabbitMQ
            var rabbitOptions = new RabbitMQOptions();
            //Читаем опции из файла и биндим конфигурацию к переменной
            builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(RabbitMQOptions.Options));
            builder.Configuration.GetSection(RabbitMQOptions.Options).Bind(rabbitOptions);
            //Создаём BackgroundService для прослушивания входящих сообщений и передаем ему опции
          // // builder.Services.AddSingleton<IHostedService>(x =>
          // // ActivatorUtilities.CreateInstance<RabbitMQConsumerChat>(x, rabbitOptions)
          // // );          
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:6001", "http://localhost:5001")
                            .AllowAnyHeader()
                            .WithMethods("GET", "POST")
                            .AllowCredentials();
                    });
            });
           
            var app = builder.Build();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();
            app.MapHub<ChatHub>("/chatHub");
  
            app.MapGet("/", () => "<h3>Server Start!</h3>");

            app.Run();
        }
    }
}