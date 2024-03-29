﻿using System.Net.Mime;
using Auth.Providers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using View.Models.In;
using View.Models.Out;

namespace Auth.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[Route("login"), ApiController]
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
    
    [HttpPut]
    [ProducesResponseType(typeof(OutUserView), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshAsync([FromBody]InRefreshTokenView model)
    {
        var userResult = await _userProvider.RefreshAsync(model.Token);
        return userResult;
    }
}
