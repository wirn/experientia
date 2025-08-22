using Experientia.Api.Data;
using Experientia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Experientia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(AppDbContext db) : ControllerBase
{
    // Small read DTOs to shape GET responses nicely
    public record TechRead(int Id, string Name);
    public record JobRead(
        long Id, long UserId, string Place, string Role, string? Location,
        DateOnly FromDate, DateOnly? ToDate, List<TechRead> Techniques);

    // GET: api/jobs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobRead>>> GetAll()
    {
        var items = await db.Jobs
            .AsNoTracking()
            .Include(j => j.JobTechniques)
                .ThenInclude(jt => jt.Technique)
            .Select(j => new JobRead(
                j.Id, j.UserId, j.Place, j.Role, j.Location, j.FromDate, j.ToDate,
                j.JobTechniques
                    .OrderBy(jt => jt.Technique.Name)
                    .Select(jt => new TechRead(jt.TechniqueId, jt.Technique.Name))
                    .ToList()))
            .ToListAsync();

        return Ok(items);
    }

    // GET: api/jobs/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<JobRead>> GetOne(long id)
    {
        var job = await db.Jobs
            .AsNoTracking()
            .Include(j => j.JobTechniques)
                .ThenInclude(jt => jt.Technique)
            .Where(j => j.Id == id)
            .Select(j => new JobRead(
                j.Id, j.UserId, j.Place, j.Role, j.Location, j.FromDate, j.ToDate,
                j.JobTechniques
                    .OrderBy(jt => jt.Technique.Name)
                    .Select(jt => new TechRead(jt.TechniqueId, jt.Technique.Name))
                    .ToList()))
            .FirstOrDefaultAsync();

        return job is null ? NotFound() : Ok(job);
    }

    // POST: api/jobs
    [HttpPost]
    public async Task<ActionResult<JobRead>> Create(JobUpsertDto dto)
    {
        // Validate user exists
        var userExists = await db.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists) return BadRequest("UserId does not exist.");

        // Validate technique ids exist (optional but helpful)
        var incoming = dto.TechniqueIds.Distinct().ToList();
        var existingIds = await db.Techniques
            .Where(t => incoming.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();
        if (existingIds.Count != incoming.Count)
            return BadRequest("One or more TechniqueIds do not exist.");

        var job = new Job
        {
            UserId = dto.UserId,
            Place = dto.Place,
            Role = dto.Role,
            Location = dto.Location,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate,
            JobTechniques = incoming.Select(id => new JobTechnique
            {
                TechniqueId = id
            }).ToList()
        };

        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        // Return shaped object
        var read = await db.Jobs.AsNoTracking()
            .Include(j => j.JobTechniques).ThenInclude(jt => jt.Technique)
            .Where(j => j.Id == job.Id)
            .Select(j => new JobRead(
                j.Id, j.UserId, j.Place, j.Role, j.Location, j.FromDate, j.ToDate,
                j.JobTechniques
                    .OrderBy(jt => jt.Technique.Name)
                    .Select(jt => new TechRead(jt.TechniqueId, jt.Technique.Name))
                    .ToList()))
            .FirstAsync();

        return CreatedAtAction(nameof(GetOne), new { id = read.Id }, read);
    }

    // PUT: api/jobs/5
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, JobUpsertDto dto)
    {
        var job = await db.Jobs
            .Include(j => j.JobTechniques) // load current links
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null) return NotFound();

        // (optional) validate user/techniques exist
        if (!await db.Users.AnyAsync(u => u.Id == dto.UserId))
            return BadRequest("UserId does not exist.");

        var incoming = dto.TechniqueIds.Distinct().ToHashSet();
        var existing = job.JobTechniques.Select(jt => jt.TechniqueId).ToHashSet();

        var toAdd = incoming.Except(existing).ToList();
        var toRemove = existing.Except(incoming).ToList();

        // update scalar fields
        job.UserId = dto.UserId;
        job.Place = dto.Place;
        job.Role = dto.Role;
        job.Location = dto.Location;
        job.FromDate = dto.FromDate;
        job.ToDate = dto.ToDate;

        // add new links (no need to Attach Technique; we only set FK)
        foreach (var techId in toAdd)
            job.JobTechniques.Add(new JobTechnique { JobId = job.Id, TechniqueId = techId });

        // remove links
        if (toRemove.Count > 0)
        {
            var rows = job.JobTechniques.Where(jt => toRemove.Contains(jt.TechniqueId)).ToList();
            foreach (var jt in rows)
                job.JobTechniques.Remove(jt);
        }

        await db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/jobs/5
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var job = await db.Jobs.FindAsync(id);
        if (job is null) return NotFound();

        db.Jobs.Remove(job); // JobTechnique rows are deleted by cascade
        await db.SaveChangesAsync();
        return NoContent();
    }

    // (nice-to-have) GET: api/jobs/by-user/123
    [HttpGet("by-user/{userId:long}")]
    public async Task<ActionResult<IEnumerable<JobRead>>> GetByUser(long userId)
    {
        var items = await db.Jobs
            .AsNoTracking()
            .Where(j => j.UserId == userId)
            .Include(j => j.JobTechniques).ThenInclude(jt => jt.Technique)
            .Select(j => new JobRead(
                j.Id, j.UserId, j.Place, j.Role, j.Location, j.FromDate, j.ToDate,
                j.JobTechniques
                    .OrderBy(jt => jt.Technique.Name)
                    .Select(jt => new TechRead(jt.TechniqueId, jt.Technique.Name))
                    .ToList()))
            .ToListAsync();

        return Ok(items);
    }
}
