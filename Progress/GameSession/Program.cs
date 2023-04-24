using GameSession.Hubs;
using GameSession.Models;
using GameSession.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
        }));

builder.Services.AddSingleton<GameManager>();
builder.Services.AddSingleton<GameStatisticBrokerClient>();

builder.Services.Configure<UserStatisticIntegrationOption>(
    builder.Configuration.GetSection(nameof(UserStatisticIntegrationOption)));

var app = builder.Build();

app.MapControllers();

app.UseCors("CorsPolicy");
app.MapHub<GameHub>("/gameHub");

app.Run();
