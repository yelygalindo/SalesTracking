namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectVisits;

internal static class ProjectVisitQueries
{
    public const string GetProjectId = @"
SELECT TOP 1 Id
FROM Projects
WHERE ExternalId = @ProjectExternalId
  AND IsDeleted = 0;";

    public const string Get = @"
SELECT
    JSON_VALUE(pt.MetadataJson, '$.externalId') AS ExternalId,
    pt.OccurredAtUtc AS VisitedAtUtc,
    TRY_CONVERT(decimal(9, 6), JSON_VALUE(pt.MetadataJson, '$.latitude')) AS Latitude,
    TRY_CONVERT(decimal(9, 6), JSON_VALUE(pt.MetadataJson, '$.longitude')) AS Longitude,
    JSON_VALUE(pt.MetadataJson, '$.notes') AS Notes,
    JSON_VALUE(pt.MetadataJson, '$.result') AS Result,
    u.ExternalId AS SellerExternalId,
    u.FullName AS SellerName
FROM dbo.ProjectTimeline pt
INNER JOIN Users u ON u.Id = pt.CreatedByUserId
WHERE pt.ProjectId = @ProjectId
  AND pt.EventTypeId = 5
  AND pt.IsDeleted = 0
  AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
  AND (@FromUtc IS NULL OR pt.OccurredAtUtc >= @FromUtc)
  AND (@ToUtc IS NULL OR pt.OccurredAtUtc <= @ToUtc)
ORDER BY pt.OccurredAtUtc DESC;";
}
