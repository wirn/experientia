


using Experientia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Experientia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    public UsersController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    // DTOs (enkla, kan flyttas till egen mapp/fil)
    public record RegisterDto(string UserName, string Email, string Password, string? FullName);
    public record UpdateUserDto(string UserName, string Email, string? FullName);
    public record UserViewDto(long Id, string? UserName, string? Email, string? FullName);

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserViewDto>>> GetAll()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .Select(u => new UserViewDto(u.Id, u.UserName!, u.Email!, u.FullName))
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/users/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserViewDto>> GetOne(long id)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserViewDto(u.Id, u.UserName!, u.Email!, u.FullName))
            .FirstOrDefaultAsync();

        return user is null ? NotFound() : Ok(user);
    }

    // POST: api/users/register
    [HttpPost("register")]
    public async Task<ActionResult<UserViewDto>> Register(RegisterDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName ?? ""
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var view = new UserViewDto(user.Id, user.UserName!, user.Email!, user.FullName);
        return CreatedAtAction(nameof(GetOne), new { id = user.Id }, view);
    }

    // PUT: api/users/5
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.FullName = dto.FullName ?? "";

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

    // DELETE: api/users/5
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }
}

//public class UsersController(AppDbContext db) : ControllerBase
//{
//    // GET: api/users
//    [HttpGet]
//    public async Task<ActionResult<IEnumerable<AppUser>>> GetAll()
//        => await db.Users
//                   .AsNoTracking()
//                   .ToListAsync();

//    // GET: api/users/5
//    [HttpGet("{id:long}")]
//    public async Task<ActionResult<AppUser>> GetOne(long id)
//    {
//        var user = await db.Users
//                           .AsNoTracking()
//                           .FirstOrDefaultAsync(u => u.Id == id);
//        return user is null ? NotFound() : Ok(user);
//    }

//    // POST: api/users
//    [HttpPost]
//    public async Task<ActionResult<AppUser>> Create(AppUser user)
//    {
//        if (!ModelState.IsValid) return ValidationProblem(ModelState);

//        db.Users.Add(user);
//        await db.SaveChangesAsync();
//        return CreatedAtAction(nameof(GetOne), new { id = user.Id }, user);
//    }

//    // PUT: api/users/5
//    [HttpPut("{id:long}")]
//    public async Task<IActionResult> Update(long id, AppUser updated)
//    {
//        if (id != updated.Id) return BadRequest();

//        var exists = await db.Users.AnyAsync(u => u.Id == id);
//        if (!exists) return NotFound();


//        db.Entry(updated).Property(x => x.FullName).IsModified = true;
//        db.Entry(updated).Property(x => x.Email).IsModified = true;

//        await db.SaveChangesAsync();
//        return NoContent();
//    }

//    // DELETE: api/users/5
//    [HttpDelete("{id:long}")]
//    public async Task<IActionResult> Delete(long id)
//    {
//        var user = await db.Users.FindAsync(id);
//        if (user is null) return NotFound();

//        db.Users.Remove(user);
//        await db.SaveChangesAsync();
//        return NoContent();
//    }
//}
