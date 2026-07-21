namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline
{
    internal sealed class ProjectTimelineEvent
    {
        public int ProjectId { get; set; }
        public int EventTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
    }
}