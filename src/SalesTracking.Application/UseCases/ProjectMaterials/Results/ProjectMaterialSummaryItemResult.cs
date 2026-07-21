namespace SalesTracking.Application.UseCases.ProjectMaterials.Results;

public sealed class ProjectMaterialSummaryItemResult
{
    public string ProductExternalId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CommittedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal PendingQuantity { get; set; }
}
