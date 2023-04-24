using LogService;


var logConsumer = new LogConsumer();
logConsumer.SetUpLogger();
logConsumer.RunRabbitMQLogConcumer();

