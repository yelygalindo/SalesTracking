namespace SalesTracking.Infrastructure.Persistence.Sql.Reports
{
    internal static class ReportRepositoryQueries
    {
        public const string GetDeliveries = @"
SELECT
    d.ExternalId AS DeliveryExternalId,
    p.ExternalId AS ProjectExternalId,
    p.Name AS ProjectName,
    c.ExternalId AS CustomerExternalId,
    c.Name AS CustomerName,
    u.ExternalId AS SellerExternalId,
    u.FullName AS SellerName,
    d.StatusId,
    ds.Name AS StatusName,
    d.CommittedDateUtc,
    d.DeliveredDateUtc,
    COALESCE(SUM(di.Quantity), 0) AS TotalQuantity,
    COALESCE(SUM(di.DeliveredQuantity), 0) AS DeliveredQuantity,
    d.Notes,
    d.CreatedAtUtc,
    d.UpdatedAtUtc,
    COUNT(1) OVER() AS TotalCount
FROM Deliveries d
INNER JOIN Projects p ON p.Id = d.ProjectId
LEFT JOIN Customers c ON c.Id = p.CustomerId
INNER JOIN Users u ON u.Id = d.SellerId
INNER JOIN DeliveryStatus ds ON ds.DeliveryStatusId = d.StatusId
LEFT JOIN DeliveryItems di ON di.DeliveryId = d.Id AND di.IsDeleted = 0
WHERE d.IsDeleted = 0
  AND p.IsDeleted = 0
  AND (@From IS NULL OR d.CommittedDateUtc >= @From)
  AND (@To IS NULL OR d.CommittedDateUtc <= @To)
  AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
  AND (@StatusId IS NULL OR d.StatusId = @StatusId)
GROUP BY
    d.ExternalId,
    p.ExternalId,
    p.Name,
    c.ExternalId,
    c.Name,
    u.ExternalId,
    u.FullName,
    d.StatusId,
    ds.Name,
    d.CommittedDateUtc,
    d.DeliveredDateUtc,
    d.Notes,
    d.CreatedAtUtc,
    d.UpdatedAtUtc
ORDER BY d.CommittedDateUtc DESC, d.CreatedAtUtc DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        public const string GetCustomersPendingContact = @"
SELECT
    r.ExternalId AS ReminderExternalId,
    c.ExternalId AS CustomerExternalId,
    c.Name AS CustomerName,
    c.CompanyName,
    c.Phone,
    c.Email,
    c.StatusId,
    seller.ExternalId AS SellerExternalId,
    seller.FullName AS SellerName,
    r.Text,
    r.ReminderAtUtc,
    assigned.ExternalId AS AssignedToExternalId,
    assigned.FullName AS AssignedToName,
    COUNT(1) OVER() AS TotalCount
FROM CustomerReminders r
INNER JOIN Customers c ON c.Id = r.CustomerId
LEFT JOIN Users seller ON seller.Id = c.SellerId
INNER JOIN Users assigned ON assigned.Id = r.AssignedToId
WHERE c.IsDeleted = 0
  AND r.Completed = 0
  AND (@From IS NULL OR r.ReminderAtUtc >= @From)
  AND (@To IS NULL OR r.ReminderAtUtc <= @To)
  AND (@SellerExternalId IS NULL OR seller.ExternalId = @SellerExternalId)
  AND (@StatusId IS NULL OR c.StatusId = @StatusId)
ORDER BY r.ReminderAtUtc ASC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        public const string GetProjects = @"
SELECT
    p.ExternalId AS ProjectExternalId,
    p.Name,
    p.Description,
    c.ExternalId AS CustomerExternalId,
    c.Name AS CustomerName,
    u.ExternalId AS SellerExternalId,
    u.FullName AS SellerName,
    p.StatusId,
    ps.Name AS StatusName,
    p.EstimatedAmount,
    p.StartDateUtc,
    p.ExpectedCloseDateUtc,
    p.ActualCloseDateUtc,
    p.ProgressPercentage,
    p.Address,
    p.Latitude,
    p.Longitude,
    p.CreatedAtUtc,
    p.UpdatedAtUtc,
    COUNT(1) OVER() AS TotalCount
FROM Projects p
INNER JOIN ProjectStatus ps ON ps.ProjectStatusId = p.StatusId
INNER JOIN Customers c ON c.Id = p.CustomerId
INNER JOIN Users u ON u.Id = p.SellerId
WHERE p.IsDeleted = 0
  AND (@From IS NULL OR p.CreatedAtUtc >= @From)
  AND (@To IS NULL OR p.CreatedAtUtc <= @To)
  AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
  AND (@StatusId IS NULL OR p.StatusId = @StatusId)
ORDER BY p.CreatedAtUtc DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        public const string GetCommercialActivity = @"
SELECT
    pt.ExternalId AS TimelineExternalId,
    p.ExternalId AS ProjectExternalId,
    p.Name AS ProjectName,
    p.StatusId AS ProjectStatusId,
    ps.Name AS ProjectStatusName,
    seller.ExternalId AS SellerExternalId,
    seller.FullName AS SellerName,
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
    createdBy.ExternalId AS CreatedByExternalId,
    createdBy.FullName AS CreatedByName,
    pt.RelatedEntityType,
    pt.RelatedEntityId,
    pt.MetadataJson,
    COUNT(1) OVER() AS TotalCount
FROM ProjectTimeline pt
INNER JOIN Projects p ON p.Id = pt.ProjectId
INNER JOIN ProjectStatus ps ON ps.ProjectStatusId = p.StatusId
LEFT JOIN Users seller ON seller.Id = p.SellerId
LEFT JOIN Users createdBy ON createdBy.Id = pt.CreatedByUserId
WHERE pt.IsDeleted = 0
  AND p.IsDeleted = 0
  AND (@From IS NULL OR pt.OccurredAtUtc >= @From)
  AND (@To IS NULL OR pt.OccurredAtUtc <= @To)
  AND (@SellerExternalId IS NULL OR seller.ExternalId = @SellerExternalId)
  AND (@StatusId IS NULL OR p.StatusId = @StatusId)
ORDER BY pt.OccurredAtUtc DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
    }
}
