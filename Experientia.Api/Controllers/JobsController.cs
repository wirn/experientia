using Experientia.Api.Data;
using Experientia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Experientia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Job>> GetAll() =>
        await db.Jobs.Include(j => j.Techniques).AsNoTracking().ToListAsync();

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Job>> GetOne(long id)
        => await db.Jobs.Include(j => j.Techniques).AsNoTracking()
               .FirstOrDefaultAsync(j => j.Id == id) is { } j ? Ok(j) : NotFound();

    [HttpPost]
    public async Task<ActionResult<Job>> Create(JobUpsertDto dto)
    {
        var techs = await db.Techniques.Where(t => dto.TechniqueIds.Contains(t.Id)).ToListAsync();
        var job = new Job
        {
            UserId = dto.UserId,
            Place = dto.Place,
            Role = dto.Role,
            Location = dto.Location,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate,
            Techniques = techs
        };
        db.Jobs.Add(job);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = job.Id }, job);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, JobUpsertDto dto)
    {
        var job = await db.Jobs
            .Include(j => j.Techniques)        // viktigt: spåra relationerna för just detta Job
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null) return NotFound();

        // Basfält
        job.UserId = dto.UserId;
        job.Place = dto.Place;
        job.Role = dto.Role;
        job.Location = dto.Location;
        job.FromDate = dto.FromDate;
        job.ToDate = dto.ToDate;

        // --- DIFFERENTIAL UPDATE AV MANY-TO-MANY ---
        var currentIds = job.Techniques.Select(t => t.Id).ToHashSet();
        var newIds = (dto.TechniqueIds ?? new List<int>()).ToHashSet();

        var toAddIds = newIds.Except(currentIds).ToList();
        var toRemoveIds = currentIds.Except(newIds).ToList();

        if (toAddIds.Count > 0)
        {
            var toAdd = await db.Techniques
                .Where(t => toAddIds.Contains(t.Id))
                .ToListAsync();
            foreach (var t in toAdd)
                job.Techniques.Add(t);         // lägger BARA till länkar för DETTA jobb
        }

        if (toRemoveIds.Count > 0)
        {
            // ta bort länkar bara för detta jobb
            job.Techniques.RemoveAll(t => toRemoveIds.Contains(t.Id));
        }
        // -------------------------------------------

        await db.SaveChangesAsync();
        return NoContent();
    }
    //public async Task<IActionResult> Update(long id, JobUpsertDto dto)
    //{
    //    var job = await db.Jobs.Include(j => j.Techniques).FirstOrDefaultAsync(j => j.Id == id);
    //    if (job is null) return NotFound();

    //    job.Place = dto.Place; job.Role = dto.Role; job.Location = dto.Location;
    //    job.FromDate = dto.FromDate; job.ToDate = dto.ToDate; job.UserId = dto.UserId;

    //    // uppdatera many-to-many
    //    job.Techniques.Clear();
    //    var techs = await db.Techniques.Where(t => dto.TechniqueIds.Contains(t.Id)).ToListAsync();
    //    foreach (var t in techs) job.Techniques.Add(t);

    //    await db.SaveChangesAsync();
    //    return NoContent();
    //}

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var job = await db.Jobs.FindAsync(id);
        if (job is null) return NotFound();
        db.Remove(job);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
