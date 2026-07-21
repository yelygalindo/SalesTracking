namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectMaterials.Rows;

internal sealed class ProjectMaterialSummaryRow
{
    public string ProductExternalId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CommittedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal PendingQuantity { get; set; }
}
