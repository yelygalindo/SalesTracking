namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectVisits.Rows;

internal sealed class ProjectVisitRow
{
    public string ExternalId { get; set; } = string.Empty;
    public DateTime VisitedAtUtc { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Notes { get; set; }
    public string Result { get; set; } = string.Empty;
    public string SellerExternalId { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
}
