namespace UrbanTrack.Api.Controllers.Responses.ProjectMaterials;

public sealed class ProjectMaterialsSummaryResponse
{
    public IReadOnlyList<ProjectMaterialSummaryItemResponse> Items { get; set; } = [];
}
