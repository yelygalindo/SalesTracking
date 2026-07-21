namespace SalesTracking.Application.UseCases.ProjectVisits.Models;

public sealed class CreateProjectVisit
{
    public string ExternalId { get; set; } = string.Empty;
    public string ProjectExternalId { get; set; } = string.Empty;
    public DateTime VisitedAtUtc { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Notes { get; set; }
    public string Result { get; set; } = string.Empty;
    public int SellerUserId { get; set; }
}
