namespace UrbanTrack.Api.Controllers.Responses.ProjectTimeline
{
    public sealed class ProjectTimelineResponse
    {
        public string ExternalId { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public ProjectTimelineUserResponse? CreatedBy { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? MetadataJson { get; set; }
    }
}