namespace SalesTracking.Application.UseCases.CustomerTimeline.Results;

public sealed class CustomerTimelineResult
{
    public string ExternalId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public CustomerTimelineUserResult? CreatedBy { get; set; }
}

public sealed class CustomerTimelineUserResult
{
    public string ExternalId { get; set; } = string.Empty;
    public string? Name { get; set; }
}
