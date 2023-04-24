using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Entity;
using System.Text;
using UserStatistic.API;
using RabbitMQLibrary.RabbitMQ.Abstract;
using RabbitMQLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen();

string connstr = builder.Configuration.GetConnectionString("CORE_CONNECTION_STRING");
UserStatisticAPI._connstr = connstr;

builder.Services.AddDbContext<DatabaseContext>(option =>
{
    option.UseNpgsql(connstr);
    option.EnableDetailedErrors();
});

var rabbitOptions = new RabbitMQOptions();
//Читаем опции из файла и биндим конфигурацию к переменной
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(RabbitMQOptions.Options));
builder.Configuration.GetSection(RabbitMQOptions.Options).Bind(rabbitOptions);
//Создаём BackgroundService для прослушивания входящих сообщений и передаем ему опции
builder.Services.AddSingleton<IHostedService>(x =>
ActivatorUtilities.CreateInstance<RabbitMQConsumer>(x, rabbitOptions)
);


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
