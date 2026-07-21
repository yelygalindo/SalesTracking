namespace UrbanTrack.Api.Controllers.Requests.ProjectVisits;

public sealed class CreateProjectVisitRequest
{
    public DateTimeOffset VisitedAtUtc { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Notes { get; set; }
    public string Result { get; set; } = string.Empty;
}
