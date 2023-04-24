using Auth.Providers.Interfaces;
using Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using AutoMapper;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using View.Models.In;
using View.Models.Out;

namespace Auth.Providers;

public class UserProvider:IUserProvider
{
    private readonly DatabaseContext _context;
    private readonly ILogger<UserProvider> _logger;
    private readonly IMapper _mapper;
    
    private readonly int _authTokenLifetime;
    private readonly string _authSecret;

    public UserProvider(DatabaseContext context, ILogger<UserProvider> logger, IConfiguration configuration, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
        // время жизни авторизационного токена
        _authTokenLifetime = configuration.GetValue("Settings:AuthLifetime", 3*60);
        _authSecret = configuration["Settings:AuthenticationSecret"] ?? string.Empty;
    }

    public async Task<IActionResult> AuthenticateAsync(InAuthView userView, CancellationToken stoppingToken)
    {
        if (string.IsNullOrEmpty(userView.Email) || string.IsNullOrEmpty(userView.Password)) 
            return new BadRequestObjectResult("Логин и пароль должны быть заполнены");
        
        var userDb = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == userView.Email, stoppingToken);
        if (userDb == null)
            return new NotFoundObjectResult($"Пользователь с логином '{userView.Email}' не найден");
        
        var correctPassword = VerifyPasswordHash(userView.Password, userDb);
        if (!correctPassword)
            return new BadRequestObjectResult("Неверный пароль");
        
        var user = _mapper.Map<OutUserView>(userDb);
        user.Token = GenerateAccessToken(userDb);

        return new OkObjectResult(user);
    }
    
    public async Task<IActionResult> CreateAsync(InCreateUserView userView,
        CancellationToken stoppingToken = default)
    {
        if (string.IsNullOrWhiteSpace(userView.Password))
            return new BadRequestObjectResult("Пароль не указан!");
        // TODO: Полин сделай проверку на корректность введенного e-mail
        if (await _context.Users.AsNoTracking().AnyAsync(x => x.Email == userView.Email, cancellationToken: stoppingToken))
            return new ConflictObjectResult("Пользователь с такой почтой \"" + userView.Email + "\" уже зарегистрирован");

        if (string.IsNullOrEmpty(userView.Name)) userView.Name = userView.Email;    
        
        var (passwordHash, passwordSalt) = CreatePasswordHash(userView.Password);
        var userDb = _mapper.Map<User>(userView);
        userDb.PasswordHash = passwordHash;
        userDb.PasswordSalt = passwordSalt;
        _context.Users.Add(userDb);
        await _context.SaveChangesAsync(stoppingToken);

        return new OkObjectResult(userDb);
    }

    #region Helpers

    private bool VerifyPasswordHash(string password, User userDb)
    {
        if (userDb.PasswordHash.Length != 64)
        {
            _logger.LogError("Неверная длина хэша (должно быть 64 байта) у пользователя {UserDbEmail}", userDb.Email);
            return false;
        }

        if (userDb.PasswordSalt.Length != 128)
        {
            _logger.LogError("Неверная длина соли (должно быть 128 байт) у пользователя {UserDbEmail}", userDb.Email);
            return false;
        }

        using var hmac = new HMACSHA512(userDb.PasswordSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return !computedHash.Where((t, i) => t != userDb.PasswordHash[i]).Any();
    }

    private string GenerateAccessToken(User userDb)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authSecret);
        var claims = new List<Claim>
        {
            new(nameof(userDb.Id), userDb.Id.ToString())
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new GenericIdentity(userDb.Name),
                claims.ToArray()
            ),
            Expires = DateTime.Now.AddMinutes(_authTokenLifetime),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private static (byte[], byte[]) CreatePasswordHash(string password)
    {
        byte[] passwordHash;
        byte[] passwordSalt;

        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        return (passwordHash, passwordSalt);
    }
    #endregion
}