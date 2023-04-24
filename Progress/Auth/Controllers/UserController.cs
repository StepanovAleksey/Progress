using System.Net.Mime;
using System.Text.Json;
using Auth.Providers.Interfaces;
using Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQLibrary;
using RabbitMQLibrary.RabbitMQ.Abstract;
using View.Models.In;
using View.Models.Out;

namespace Auth.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[Route("api/[controller]"), ApiController]
public class UserController: ControllerBase
{
    private readonly IUserProvider _userProvider;
    public UserController(IUserProvider userProvider)
    {
        _userProvider = userProvider;
    }
    
    [HttpPost, AllowAnonymous]
    [ProducesResponseType(typeof(OutUserView), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] InCreateUserView userView)
    {
        var userResult = await _userProvider.CreateAsync(userView);
        if (((ObjectResult)userResult).StatusCode != 200)
            return userResult;

        User newuser = (User)((ObjectResult)userResult).Value;
        if (newuser != null)
        {
            InUserStatistics userStatistic = new InUserStatistics();
            userStatistic.UserId = newuser.Id;
            userStatistic.WinGames = 0;
            userStatistic.LossGames = 0;
            userStatistic.GameCount = 0;

            IRabbitMQOptions options = new RabbitMQOptions()
            {
                UserName = "iyglhuaj",
                Password = "AtkBM4d1ytSJbIIMVtj6uVjdoBN4tz-O",
                VirtualHost = "iyglhuaj",
                HostName = "sparrow.rmq.cloudamqp.com",
                QueueName = { "UserStatistic" }
            };
            IRabbitMQPubliserService mqService = new RabbitMQPubliserService(options);
            var usrtxt = JsonSerializer.Serialize(userStatistic);
            mqService.SendMessage(usrtxt);
        }

        return userResult;
    }
    
}