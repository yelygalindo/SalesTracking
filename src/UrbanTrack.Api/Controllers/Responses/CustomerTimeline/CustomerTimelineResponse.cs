namespace UrbanTrack.Api.Controllers.Responses.CustomerTimeline;

public sealed class CustomerTimelineResponse
{
    public string ExternalId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public CustomerTimelineUserResponse? CreatedBy { get; set; }
}

public sealed class CustomerTimelineUserResponse
{
    public string ExternalId { get; set; } = string.Empty;
    public string? Name { get; set; }
}
