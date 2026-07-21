namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline.Rows
{
    internal sealed class ProjectTimelineRow
    {
        public string ExternalId { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public string? CreatedByUserExternalId { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? MetadataJson { get; set; }
        public int TotalCount { get; set; }
    }
}