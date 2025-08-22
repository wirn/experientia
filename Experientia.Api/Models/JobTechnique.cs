namespace Experientia.Api.Models;

public class JobTechnique
{
    public long JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int TechniqueId { get; set; }
    public Technique Technique { get; set; } = null!;
}
