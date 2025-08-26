using Experientia.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Experientia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;
    private readonly IConfiguration _cfg;

    public AuthController(UserManager<AppUser> users, SignInManager<AppUser> signIn, IConfiguration cfg)
    {
        _users = users;
        _signIn = signIn;
        _cfg = cfg;
    }

    public record RegisterDto(string Email, string FullName, string Password);
    public record LoginDto(string Email, string Password);

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var exists = await _users.FindByEmailAsync(dto.Email);
        if (exists is not null) return BadRequest("Email already in use.");

        var user = new AppUser { UserName = dto.Email, Email = dto.Email, FullName = dto.FullName };
        var result = await _users.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors.Select(e => e.Description));

        return Ok(new { message = "Registered" });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized("Invalid credentials");

        var valid = await _users.CheckPasswordAsync(user, dto.Password);
        if (!valid) return Unauthorized("Invalid credentials");

        var token = GenerateJwt(user);
        return Ok(new
        {
            accessToken = token,
            user = new { id = user.Id, email = user.Email, fullName = user.FullName }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null) return Unauthorized();

        var user = await _users.FindByIdAsync(id);
        if (user is null) return Unauthorized();

        return Ok(new { id = user.Id, email = user.Email, fullName = user.FullName });
    }

    private string GenerateJwt(AppUser user)
    {
        var jwt = _cfg.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName ?? user.Email ?? ""),
            new(ClaimTypes.Email, user.Email ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
