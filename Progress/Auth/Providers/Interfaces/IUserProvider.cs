using Microsoft.AspNetCore.Mvc;
using View.Models.In;
using View.Models.Out;

namespace Auth.Providers.Interfaces;

public interface IUserProvider
{
    Task<IActionResult> AuthenticateAsync(InAuthView userView, CancellationToken stoppingToken = default);
    Task<IActionResult> CreateAsync(InCreateUserView userView, CancellationToken stoppingToken = default);
}