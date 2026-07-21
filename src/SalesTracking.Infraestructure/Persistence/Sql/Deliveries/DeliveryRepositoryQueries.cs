namespace SalesTracking.Infrastructure.Persistence.Sql.Deliveries
{
    internal static class DeliveryRepositoryQueries
    {
        public const string GetStatuses = @"
SELECT
    DeliveryStatusId,
    Name,
    Description,
    IsActive
FROM DeliveryStatus
WHERE IsActive = 1
ORDER BY DeliveryStatusId;";

        public const string Get = @"
SELECT
    d.Id,
    d.ExternalId,
    p.ExternalId AS ProjectExternalId,
    p.Name AS ProjectName,
    u.ExternalId AS SellerExternalId,
    u.FullName AS SellerName,
    d.StatusId,
    ds.Name AS StatusName,
    d.CommittedDateUtc,
    d.DeliveredDateUtc,
    d.Notes,
    d.CreatedAtUtc,
    d.UpdatedAtUtc,
    COUNT(1) OVER() AS TotalCount
FROM Deliveries d
INNER JOIN Projects p ON p.Id = d.ProjectId
LEFT JOIN Customers c ON c.Id = p.CustomerId
INNER JOIN Users u ON u.Id = d.SellerId
INNER JOIN DeliveryStatus ds ON ds.DeliveryStatusId = d.StatusId
WHERE d.IsDeleted = 0
  AND (@ProjectExternalId IS NULL OR p.ExternalId = @ProjectExternalId)
  AND (@CustomerExternalId IS NULL OR c.ExternalId = @CustomerExternalId)
  AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
  AND (@StatusId IS NULL OR d.StatusId = @StatusId)
  AND (@FromUtc IS NULL OR d.CommittedDateUtc >= @FromUtc)
  AND (@ToUtc IS NULL OR d.CommittedDateUtc <= @ToUtc)
  AND (
      @Overdue IS NULL
      OR (@Overdue = 1 AND d.StatusId <> 3 AND d.CommittedDateUtc < SYSUTCDATETIME())
      OR (@Overdue = 0 AND (d.StatusId = 3 OR d.CommittedDateUtc >= SYSUTCDATETIME()))
  )
ORDER BY d.CreatedAtUtc DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;";

        public const string GetByExternalId = @"
SELECT TOP 1
    d.Id,
    d.ExternalId,
    p.ExternalId AS ProjectExternalId,
    p.Name AS ProjectName,
    u.ExternalId AS SellerExternalId,
    u.FullName AS SellerName,
    d.StatusId,
    ds.Name AS StatusName,
    d.CommittedDateUtc,
    d.DeliveredDateUtc,
    d.Notes,
    d.CreatedAtUtc,
    d.UpdatedAtUtc
FROM Deliveries d
INNER JOIN Projects p ON p.Id = d.ProjectId
INNER JOIN Users u ON u.Id = d.SellerId
INNER JOIN DeliveryStatus ds ON ds.DeliveryStatusId = d.StatusId
WHERE d.IsDeleted = 0
  AND d.ExternalId = @ExternalId;";

        public const string GetItemsByDeliveryIds = @"
SELECT
    di.DeliveryId,
    di.ExternalId,
    p.ExternalId AS ProductExternalId,
    p.Name AS ProductName,
    u.ExternalId AS UnitExternalId,
    u.Name AS UnitName,
    di.Quantity,
    di.DeliveredQuantity
FROM DeliveryItems di
INNER JOIN Products p ON p.Id = di.ProductId
INNER JOIN Units u ON u.Id = di.UnitId
WHERE di.IsDeleted = 0
  AND di.DeliveryId IN @DeliveryIds
ORDER BY di.Id;";

        public const string GetProjectInternalIdByExternalId = @"
SELECT TOP 1 Id
FROM Projects
WHERE ExternalId = @ExternalId
  AND IsDeleted = 0;";

        public const string GetSellerInternalIdByExternalId = @"
SELECT TOP 1 Id
FROM Users
WHERE ExternalId = @ExternalId
  AND IsActive = 1;";

        public const string GetProductInternalIdByExternalId = @"
SELECT TOP 1 Id
FROM Products
WHERE ExternalId = @ExternalId
  AND IsDeleted = 0;";

        public const string GetUnitInternalIdByExternalId = @"
SELECT TOP 1 Id
FROM Units
WHERE ExternalId = @ExternalId
  AND IsDeleted = 0;";

        public const string GetDeliveryInternalByExternalId = @"
SELECT TOP 1
    Id,
    ProjectId,
    SellerId,
    StatusId
FROM Deliveries
WHERE ExternalId = @ExternalId
  AND IsDeleted = 0;";

        public const string StatusExists = @"
SELECT COUNT(1)
FROM DeliveryStatus
WHERE DeliveryStatusId = @StatusId
  AND IsActive = 1;";

        public const string GetStatusName = @"
SELECT TOP 1 Name
FROM DeliveryStatus
WHERE DeliveryStatusId = @StatusId;";

        public const string Insert = @"
INSERT INTO Deliveries (
    ExternalId,
    ProjectId,
    SellerId,
    StatusId,
    CommittedDateUtc,
    DeliveredDateUtc,
    Notes,
    CreatedAtUtc,
    IsDeleted
)
OUTPUT INSERTED.Id
VALUES (
    @ExternalId,
    @ProjectId,
    @SellerId,
    @StatusId,
    @CommittedDateUtc,
    @DeliveredDateUtc,
    @Notes,
    SYSUTCDATETIME(),
    0
);";

        public const string InsertItem = @"
INSERT INTO DeliveryItems (
    ExternalId,
    DeliveryId,
    ProductId,
    UnitId,
    Quantity,
    DeliveredQuantity,
    CreatedAtUtc,
    IsDeleted
)
VALUES (
    @ExternalId,
    @DeliveryId,
    @ProductId,
    @UnitId,
    @Quantity,
    @DeliveredQuantity,
    SYSUTCDATETIME(),
    0
);";

        public const string Update = @"
UPDATE Deliveries
SET
    ProjectId = @ProjectId,
    SellerId = @SellerId,
    StatusId = @StatusId,
    CommittedDateUtc = @CommittedDateUtc,
    DeliveredDateUtc = @DeliveredDateUtc,
    Notes = @Notes,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE Id = @Id
  AND IsDeleted = 0;";

        public const string SoftDeleteItems = @"
UPDATE DeliveryItems
SET
    IsDeleted = 1,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE DeliveryId = @DeliveryId
  AND IsDeleted = 0;";

        public const string ChangeStatus = @"
UPDATE Deliveries
SET
    StatusId = @StatusId,
    DeliveredDateUtc = @DeliveredDateUtc,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE Id = @Id
  AND IsDeleted = 0;";

        public const string DeleteDelivery = @"
UPDATE Deliveries
SET
    IsDeleted = 1,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE ExternalId = @ExternalId
  AND IsDeleted = 0;";


        public const string GetReceiptItemByExternalId = @"
SELECT TOP 1
    Id,
    DeliveryId,
    ExternalId,
    Quantity,
    DeliveredQuantity
FROM DeliveryItems
WHERE ExternalId = @ExternalId
  AND DeliveryId = @DeliveryId
  AND IsDeleted = 0;";

        public const string UpdateItemDeliveredQuantity = @"
UPDATE DeliveryItems
SET
    DeliveredQuantity = @DeliveredQuantity,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE Id = @Id
  AND DeliveryId = @DeliveryId
  AND IsDeleted = 0;";

        public const string UpdateDeliveryReceiptState = @"
UPDATE Deliveries
SET
    StatusId = @StatusId,
    DeliveredDateUtc = @DeliveredDateUtc,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE Id = @Id
  AND IsDeleted = 0;";
        public const string GetQuantitiesByDeliveryId = @"
SELECT
    Quantity,
    DeliveredQuantity
FROM DeliveryItems
WHERE DeliveryId = @DeliveryId
  AND IsDeleted = 0;";
    }
}
