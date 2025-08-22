namespace Experientia.Api.Models;
public record JobUpsertDto(
    long UserId, string Place, string Role,
    string? Location, DateOnly FromDate, DateOnly? ToDate,
    List<int> TechniqueIds);