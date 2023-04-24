using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;

namespace LogService {
    public class LogProducer {

        //for console applications
        //Log.Logger = LogProducer.GetLogger();

        //for web api applications
        //builder.Logging.ClearProviders();
        //var logger = LogProducer.GetLogger();
        //builder.Logging.AddSerilog(logger);

        public static Logger GetLogger() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            return new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) => {
                clientConfiguration.Username = config.GetSection("rabbitMQ")["username"];
                clientConfiguration.Password = config.GetSection("rabbitMQ")["password"];
                clientConfiguration.Exchange = "direct_logs";
                clientConfiguration.ExchangeType = ExchangeType.Direct;
                clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
                clientConfiguration.RouteKey = "logs";
                clientConfiguration.Port = int.Parse(config.GetSection("rabbitMQ")["port"]);
                clientConfiguration.Hostnames.Add(config.GetSection("rabbitMQ")["hostname"]);
                sinkConfiguration.TextFormatter = new JsonFormatter();
            })
            .WriteTo.Console()
            .CreateLogger();
        }
    }
}
