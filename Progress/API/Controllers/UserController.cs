using System.Net.Mime;
using Entity;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using View.Models.Out;
using In = View.Models.In;
using Out = View.Models.Out;

namespace API.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[Route("user"), ApiController]
public class UserController : ControllerBase
{
    
    private readonly DatabaseContext _context;

    public UserController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Out.OutUserView>> Get(long id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (user == null)
            return NotFound();
        var result = new Out.OutUserView
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(In.InUserView inUserView)
    {
        var dbModel = new User
        {
            Name = inUserView.Name,
            Email = inUserView.Email,
            IpAdress = inUserView.IpAdress,
            Status = inUserView.Status
        };
        _context.Users.Add(dbModel);
        await _context.SaveChangesAsync();

        return Created(nameof(dbModel), new { id = dbModel.Id });
    }
}