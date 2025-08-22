using Experientia.Api.Data;
using Experientia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Experientia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TechniquesController(AppDbContext db) : ControllerBase
{
    // GET: api/techniques
    [HttpGet]
    public Task<List<Technique>> GetAll() =>
        db.Techniques.AsNoTracking().OrderBy(t => t.Name).ToListAsync();

    // GET: api/techniques/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Technique>> GetOne(int id)
        => await db.Techniques.FindAsync(id) is { } t ? Ok(t) : NotFound();

    // POST: api/techniques
    [HttpPost]
    public async Task<ActionResult<Technique>> Create(Technique t)
    {
        db.Techniques.Add(t);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = t.Id }, t);
    }

    // PUT: api/techniques/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Technique dto)
    {
        var t = await db.Techniques.FindAsync(id);
        if (t is null) return NotFound();

        t.Name = dto.Name;
        await db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/techniques/5
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
