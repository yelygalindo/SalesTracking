namespace SalesTracking.Application.UseCases.ProjectVisits.Results;

public sealed class GetProjectVisitsResult
{
    public bool Succeeded { get; set; }
    public bool NotFound { get; set; }
    public string Message { get; set; } = string.Empty;
    public IReadOnlyList<ProjectVisitResult> Visits { get; set; } = [];
}
