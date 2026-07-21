namespace SalesTracking.Infrastructure.Persistence.Sql.Dashboard
{
    internal static class DashboardQueries
    {
        public const string GetMetrics = @"
SELECT
    Prospects = (
        SELECT COUNT(1)
        FROM Customers c
        LEFT JOIN Users u ON u.Id = c.SellerId
        WHERE c.IsDeleted = 0
          AND c.StatusId = 1
          AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
    ),
    ActiveCustomers = (
        SELECT COUNT(1)
        FROM Customers c
        LEFT JOIN Users u ON u.Id = c.SellerId
        WHERE c.IsDeleted = 0
          AND c.StatusId = 2
          AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
    ),
    ActiveProjects = (
        SELECT COUNT(1)
        FROM Projects p
        INNER JOIN Users u ON u.Id = p.SellerId
        WHERE p.IsDeleted = 0
          AND p.ActualCloseDateUtc IS NULL
          AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
          AND (@StatusId IS NULL OR p.StatusId = @StatusId)
    ),
    PendingDeliveries = (
        SELECT COUNT(1)
        FROM Deliveries d
        INNER JOIN Projects p ON p.Id = d.ProjectId
        INNER JOIN Users u ON u.Id = d.SellerId
        WHERE d.IsDeleted = 0
          AND p.IsDeleted = 0
          AND d.StatusId = 1
          AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
          AND (@StatusId IS NULL OR p.StatusId = @StatusId)
    ),
    OverdueDeliveries = (
        SELECT COUNT(1)
        FROM Deliveries d
        INNER JOIN Projects p ON p.Id = d.ProjectId
        INNER JOIN Users u ON u.Id = d.SellerId
        WHERE d.IsDeleted = 0
          AND p.IsDeleted = 0
          AND d.StatusId <> 3
          AND d.CommittedDateUtc < SYSUTCDATETIME()
          AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
          AND (@StatusId IS NULL OR p.StatusId = @StatusId)
    ),
    TodayFollowUps = (
        SELECT COUNT(1)
        FROM CustomerReminders r
        INNER JOIN Customers c ON c.Id = r.CustomerId
        LEFT JOIN Users u ON u.Id = c.SellerId
        WHERE c.IsDeleted = 0
          AND r.Completed = 0
          AND CONVERT(date, r.ReminderAtUtc) = CONVERT(date, SYSUTCDATETIME())
          AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
    ),
    CompletedDeliveriesThisMonth = (
        SELECT COUNT(1)
        FROM Deliveries d
        INNER JOIN Projects p ON p.Id = d.ProjectId
        INNER JOIN Users u ON u.Id = d.SellerId
        WHERE d.IsDeleted = 0
          AND p.IsDeleted = 0
          AND d.StatusId = 3
          AND d.DeliveredDateUtc >= DATEFROMPARTS(YEAR(SYSUTCDATETIME()), MONTH(SYSUTCDATETIME()), 1)
          AND d.DeliveredDateUtc < DATEADD(month, 1, DATEFROMPARTS(YEAR(SYSUTCDATETIME()), MONTH(SYSUTCDATETIME()), 1))
          AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
          AND (@StatusId IS NULL OR p.StatusId = @StatusId)
    );";

        public const string GetProjectLocations = @"
SELECT
    p.ExternalId AS ProjectExternalId,
    p.Name,
    p.StatusId,
    ps.Name AS StatusName,
    c.Name AS CustomerName,
    u.FullName AS SellerName,
    p.ProgressPercentage,
    p.Address,
    p.Latitude,
    p.Longitude
FROM Projects p
INNER JOIN ProjectStatus ps ON ps.ProjectStatusId = p.StatusId
INNER JOIN Customers c ON c.Id = p.CustomerId
INNER JOIN Users u ON u.Id = p.SellerId
WHERE p.IsDeleted = 0
  AND p.Latitude IS NOT NULL
  AND p.Longitude IS NOT NULL
  AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
  AND (@StatusId IS NULL OR p.StatusId = @StatusId)
ORDER BY p.CreatedAtUtc DESC;";

        public const string GetRecentActivity = @"
SELECT TOP 10
    p.ExternalId AS ProjectExternalId,
    p.Name AS ProjectName,
    pt.EventTypeId,
    CASE pt.EventTypeId
        WHEN 1 THEN 'ProjectCreated'
        WHEN 2 THEN 'ProjectUpdated'
        WHEN 3 THEN 'ProjectStatusChanged'
        WHEN 4 THEN 'ProjectProgressUpdated'
        WHEN 6 THEN 'NoteAdded'
        WHEN 7 THEN 'AttachmentUploaded'
        WHEN 8 THEN 'DeliveryCreated'
        WHEN 9 THEN 'DeliveryStatusChanged'
        WHEN 10 THEN 'DeliveryReceiptConfirmed'
        ELSE 'Unknown'
    END AS EventTypeName,
    pt.Title,
    u.ExternalId AS UserExternalId,
    u.FullName AS UserName,
    pt.OccurredAtUtc
FROM ProjectTimeline pt
INNER JOIN Projects p ON p.Id = pt.ProjectId
LEFT JOIN Users u ON u.Id = pt.CreatedByUserId
WHERE pt.IsDeleted = 0
  AND p.IsDeleted = 0
  AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId OR EXISTS (
      SELECT 1 FROM Users pu WHERE pu.Id = p.SellerId AND pu.ExternalId = @SellerExternalId
  ))
  AND (@StatusId IS NULL OR p.StatusId = @StatusId)
ORDER BY pt.OccurredAtUtc DESC;";

        public const string GetUpcomingFollowUps = @"
SELECT TOP 10
    r.ExternalId AS ReminderExternalId,
    c.ExternalId AS CustomerExternalId,
    c.Name AS CustomerName,
    r.Text,
    r.ReminderAtUtc,
    u.ExternalId AS AssignedToExternalId,
    u.FullName AS AssignedToName
FROM CustomerReminders r
INNER JOIN Customers c ON c.Id = r.CustomerId
INNER JOIN Users u ON u.Id = r.AssignedToId
LEFT JOIN Users seller ON seller.Id = c.SellerId
WHERE c.IsDeleted = 0
  AND r.Completed = 0
  AND (@SellerExternalId IS NULL OR seller.ExternalId = @SellerExternalId)
ORDER BY r.ReminderAtUtc ASC;";

        public const string GetUrgentDeliveries = @"
SELECT TOP 10
    d.ExternalId AS DeliveryExternalId,
    p.ExternalId AS ProjectExternalId,
    p.Name AS ProjectName,
    d.StatusId,
    ds.Name AS StatusName,
    d.CommittedDateUtc,
    d.DeliveredDateUtc,
    CAST(CASE WHEN d.StatusId <> 3 AND d.CommittedDateUtc < SYSUTCDATETIME() THEN 1 ELSE 0 END AS bit) AS IsOverdue
FROM Deliveries d
INNER JOIN Projects p ON p.Id = d.ProjectId
INNER JOIN Users u ON u.Id = d.SellerId
INNER JOIN DeliveryStatus ds ON ds.DeliveryStatusId = d.StatusId
WHERE d.IsDeleted = 0
  AND p.IsDeleted = 0
  AND (d.StatusId IN (1, 2) OR (d.StatusId <> 3 AND d.CommittedDateUtc < SYSUTCDATETIME()))
  AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
  AND (@StatusId IS NULL OR p.StatusId = @StatusId)
ORDER BY
    CASE WHEN d.StatusId <> 3 AND d.CommittedDateUtc < SYSUTCDATETIME() THEN 0 ELSE 1 END,
    d.CommittedDateUtc ASC;";
    }
}
