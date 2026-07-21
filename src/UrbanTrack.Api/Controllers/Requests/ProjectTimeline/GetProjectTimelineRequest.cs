namespace UrbanTrack.Api.Controllers.Requests.ProjectTimeline
{
    public sealed class GetProjectTimelineRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}