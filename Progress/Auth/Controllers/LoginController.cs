using System.Net.Mime;
using System.Text.Json;
using Auth.Providers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQLibrary.RabbitMQ.Abstract;
using RabbitMQLibrary.RabbitMQ.DTO;
using RabbitMQLibrary;
using View.Models.In;
using View.Models.Out;

namespace Auth.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[Route("api/[controller]"), ApiController]
public class LoginController : ControllerBase
{
    private readonly IUserProvider _userProvider;
    public LoginController(IUserProvider userProvider)
    {
        _userProvider = userProvider;
    }
    
    [HttpPost, AllowAnonymous]
    [ProducesResponseType(typeof(OutUserView), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SignIn([FromBody] InAuthView authView)
    {
        var userResult = await _userProvider.AuthenticateAsync(authView);
        return userResult;
    }
}
