namespace Experientia.Api.Models;

public class Job
{
    public long Id { get; set; }

    public long UserId { get; set; }       // koppling till AppUser (Identity)
    public AppUser? User { get; set; }

    public string Place { get; set; } = "";
    public string Role { get; set; } = "";
    public string? Location { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly? ToDate { get; set; }

    public List<JobTechnique> JobTechniques { get; set; } = new();
}
