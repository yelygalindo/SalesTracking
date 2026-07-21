namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerTimeline;

internal static class CustomerTimelineQueries
{
    public const string GetCustomerId = @"
SELECT TOP 1 Id
FROM Customers
WHERE ExternalId = @CustomerExternalId
  AND IsDeleted = 0;";

    public const string GetTimeline = @"
SELECT
    timeline.ExternalId,
    timeline.EventType,
    timeline.Description,
    timeline.CreatedAtUtc,
    users.ExternalId AS CreatedByExternalId,
    users.FullName AS CreatedByName,
    COUNT(1) OVER() AS TotalCount
FROM CustomerTimelineEvents timeline
LEFT JOIN Users users ON users.Id = timeline.CreatedById
WHERE timeline.CustomerId = @CustomerId
ORDER BY timeline.CreatedAtUtc DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;";
}
