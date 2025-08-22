using Experientia.Api.Data;
using Experientia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Experientia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TechniquesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Technique>> Get() =>
        await db.Techniques.AsNoTracking().OrderBy(t => t.Name).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Technique>> GetOne(int id)
        => await db.Techniques.FindAsync(id) is { } t ? Ok(t) : NotFound();

    [HttpPost]
    public async Task<ActionResult<Technique>> Create(Technique t)
    {
        db.Techniques.Add(t);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = t.Id }, t);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Technique t)
    {
        if (id != t.Id) return BadRequest();
        db.Entry(t).Property(x => x.Name).IsModified = true;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await db.Techniques.FindAsync(id);
        if (t is null) return NotFound();
        db.Techniques.Remove(t);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
