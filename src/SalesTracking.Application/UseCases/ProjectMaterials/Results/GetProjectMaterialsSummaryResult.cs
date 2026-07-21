namespace SalesTracking.Application.UseCases.ProjectMaterials.Results;

public sealed class GetProjectMaterialsSummaryResult
{
    public bool Succeeded { get; set; }
    public bool NotFound { get; set; }
    public string Message { get; set; } = string.Empty;
    public IReadOnlyList<ProjectMaterialSummaryItemResult> Items { get; set; } = [];
}
