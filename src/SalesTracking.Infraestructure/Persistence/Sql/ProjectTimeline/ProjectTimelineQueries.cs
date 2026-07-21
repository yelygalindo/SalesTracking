namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline
{
    internal static class ProjectTimelineQueries
    {
        public const string Insert = @"
INSERT INTO dbo.ProjectTimeline (
    ExternalId,
    ProjectId,
    EventTypeId,
    Title,
    Description,
    OccurredAtUtc,
    CreatedByUserId,
    RelatedEntityType,
    RelatedEntityId,
    CreatedAtUtc,
    IsDeleted
)
VALUES (
    @ExternalId,
    @ProjectId,
    @EventTypeId,
    @Title,
    @Description,
    SYSUTCDATETIME(),
    @CreatedByUserId,
    @RelatedEntityType,
    @RelatedEntityId,
    SYSUTCDATETIME(),
    0
);";
    }
}