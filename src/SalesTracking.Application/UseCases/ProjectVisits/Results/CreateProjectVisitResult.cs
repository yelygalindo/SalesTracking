namespace SalesTracking.Application.UseCases.ProjectVisits.Results;

public sealed class CreateProjectVisitResult
{
    public bool Succeeded { get; set; }
    public bool NotFound { get; set; }
    public string? Id { get; set; }
    public string Message { get; set; } = string.Empty;
}
