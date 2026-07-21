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
    MetadataJson,
    CreatedAtUtc,
    IsDeleted
)
VALUES (
    @ExternalId,
    @ProjectId,
    @EventTypeId,
    @Title,
    @Description,
    COALESCE(@OccurredAtUtc, SYSUTCDATETIME()),
    @CreatedByUserId,
    @RelatedEntityType,
    @RelatedEntityId,
    @MetadataJson,
    SYSUTCDATETIME(),
    0
);";

        public const string GetProjectInternalIdByExternalId = @"
SELECT TOP 1
    Id
FROM Projects
WHERE ExternalId = @ProjectExternalId;";

        public const string GetByProjectId = @"
SELECT
    pt.ExternalId,
    pt.EventTypeId,
    CASE pt.EventTypeId
        WHEN 1 THEN 'ProjectCreated'
        WHEN 2 THEN 'ProjectUpdated'
        WHEN 3 THEN 'ProjectStatusChanged'
        WHEN 4 THEN 'ProjectProgressUpdated'
        WHEN 5 THEN 'ProjectVisited'
        WHEN 6 THEN 'NoteAdded'
        WHEN 7 THEN 'AttachmentUploaded'
        WHEN 8 THEN 'DeliveryCreated'
        WHEN 9 THEN 'DeliveryStatusChanged'
        WHEN 10 THEN 'DeliveryReceiptConfirmed'
        WHEN 11 THEN 'ProjectDeleted'
        WHEN 12 THEN 'NoteUpdated'
        WHEN 13 THEN 'NoteDeleted'
        WHEN 14 THEN 'AttachmentDeleted'
        WHEN 15 THEN 'AttachmentCoverChanged'
        WHEN 16 THEN 'DeliveryUpdated'
        WHEN 17 THEN 'DeliveryDeleted'
        WHEN 18 THEN 'DeliveryCompleted'
        ELSE 'Unknown'
    END AS EventTypeName,
    pt.Title,
    pt.Description,
    pt.OccurredAtUtc,
    u.ExternalId AS CreatedByUserExternalId,
    u.FullName AS CreatedByUserName,
    pt.RelatedEntityType,
    pt.RelatedEntityId,
    pt.MetadataJson,
    COUNT(1) OVER() AS TotalCount
FROM dbo.ProjectTimeline pt
LEFT JOIN Users u ON u.Id = pt.CreatedByUserId
WHERE pt.ProjectId = @ProjectId
  AND pt.IsDeleted = 0
ORDER BY pt.OccurredAtUtc DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;";
    }
}
