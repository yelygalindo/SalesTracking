namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerTimeline.Rows;

internal sealed class CustomerTimelineRow
{
    public string ExternalId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedByExternalId { get; set; }
    public string? CreatedByName { get; set; }
    public int TotalCount { get; set; }
}
