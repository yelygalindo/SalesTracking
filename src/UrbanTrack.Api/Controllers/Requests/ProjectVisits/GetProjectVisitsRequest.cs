namespace UrbanTrack.Api.Controllers.Requests.ProjectVisits;

public sealed class GetProjectVisitsRequest
{
    public string? SellerExternalId { get; set; }
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
}
