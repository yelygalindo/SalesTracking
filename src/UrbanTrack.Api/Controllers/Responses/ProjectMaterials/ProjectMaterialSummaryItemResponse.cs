namespace UrbanTrack.Api.Controllers.Responses.ProjectMaterials;

public sealed class ProjectMaterialSummaryItemResponse
{
    public string ProductExternalId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CommittedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal PendingQuantity { get; set; }
}
