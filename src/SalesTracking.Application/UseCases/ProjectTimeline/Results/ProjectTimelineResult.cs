namespace SalesTracking.Application.UseCases.ProjectTimeline.Results
{
    public sealed class ProjectTimelineResult
    {
        public string ExternalId { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public ProjectTimelineUserResult? CreatedBy { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? MetadataJson { get; set; }
    }
}