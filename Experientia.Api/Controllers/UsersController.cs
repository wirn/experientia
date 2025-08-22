using Experientia.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Experientia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserManager<AppUser> userManager) : ControllerBase
{
    public record RegisterDto(string Email, string Password, string FullName);
    public record UpdateUserDto(string Email, string FullName);

    // POST: api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new AppUser { UserName = dto.Email, Email = dto.Email, FullName = dto.FullName };
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { user.Id, user.Email, user.FullName });
    }

    // GET: api/users
    [HttpGet]
    public IActionResult Get()
        => Ok(userManager.Users.Select(u => new { u.Id, u.Email, u.FullName }));

    // GET: api/users/{id}
    [HttpGet("{id:long}")]
    public async Task<ActionResult<object>> GetById(long id)
    {
        var u = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (u is null) return NotFound();
        return Ok(new { u.Id, u.Email, u.FullName });
    }

    // PUT: api/users/{id}
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateUserDto dto)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        // uppdatera fält
        user.Email = dto.Email;
        user.UserName = dto.Email;     // håll UserName = Email för enkelhet
        user.FullName = dto.FullName;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return NoContent();
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return NoContent();
    }
}
