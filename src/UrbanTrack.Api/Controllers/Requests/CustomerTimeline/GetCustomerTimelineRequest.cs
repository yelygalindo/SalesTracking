namespace UrbanTrack.Api.Controllers.Requests.CustomerTimeline;

public sealed class GetCustomerTimelineRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
