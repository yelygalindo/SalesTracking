namespace SalesTracking.Infrastructure.Persistence.Sql.Projects
{
    internal static class ProjectQueries
    {
        public const string Create = @"
INSERT INTO Projects (
    ExternalId,
    Name,
    Description,
    CustomerId,
    SellerId,
    StatusId,
    EstimatedAmount,
    StartDateUtc,
    ExpectedCloseDateUtc,
    ProgressPercentage,
    ActualCloseDateUtc,
    Address,
    Latitude,
    Longitude,
    CreatedAtUtc,
    IsDeleted
)
OUTPUT INSERTED.ExternalId
SELECT
    @ExternalId,
    @Name,
    @Description,
    c.Id AS CustomerId,
    u.Id AS SellerId,
    @StatusId,
    @EstimatedAmount,
    @StartDateUtc,
    @ExpectedCloseDateUtc,
    @ProgressPercentage,
    @ActualCloseDateUtc,
    @Address,
    @Latitude,
    @Longitude,
    SYSUTCDATETIME(),
    0
FROM Customers c
INNER JOIN Users u ON u.ExternalId = @SellerExternalId AND u.IsActive = 1
WHERE c.ExternalId = @CustomerExternalId
  AND c.IsDeleted = 0;";

        public const string Get = @"
SELECT
    p.Id,    
    p.ExternalId,
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
    p.ProgressPercentage,
    p.ActualCloseDateUtc,
    p.Address,
    p.Latitude,
    p.Longitude,
    p.CreatedAtUtc,
    COUNT(1) OVER() AS TotalCount
FROM Projects p
INNER JOIN Customers c ON c.Id = p.CustomerId
INNER JOIN Users u ON u.Id = p.SellerId
INNER JOIN ProjectStatus ps ON ps.ProjectStatusId = p.StatusId
WHERE p.IsDeleted = 0
  AND (@CustomerExternalId IS NULL OR c.ExternalId = @CustomerExternalId)
  AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
  AND (@StatusId IS NULL OR p.StatusId = @StatusId)
ORDER BY p.CreatedAtUtc DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;";

        public const string GetByExternalId = @"
SELECT
    p.Id,
    p.ExternalId,
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
    p.ProgressPercentage,
    p.ActualCloseDateUtc,
    p.Address,
    p.Latitude,
    p.Longitude,
    p.CreatedAtUtc
FROM Projects p
INNER JOIN Customers c ON c.Id = p.CustomerId
INNER JOIN Users u ON u.Id = p.SellerId
INNER JOIN ProjectStatus ps ON ps.ProjectStatusId = p.StatusId
WHERE p.IsDeleted = 0
  AND p.ExternalId = @ExternalId;";

        public const string GetTimelineProjectByExternalId = @"
SELECT TOP 1
    p.Id,
    p.SellerId,
    p.StatusId,
    p.ProgressPercentage
FROM Projects p
WHERE p.IsDeleted = 0
  AND p.ExternalId = @ExternalId;";

        public const string ProjectExistsByExternalId = @"
SELECT COUNT(1)
FROM Projects p
WHERE p.IsDeleted = 0
  AND p.ExternalId = @ExternalId;";

        public const string ProjectStatusExists = @"
SELECT COUNT(1)
FROM ProjectStatus ps
WHERE ps.ProjectStatusId = @StatusId;";

        public const string CustomerExistsByExternalId = @"
SELECT COUNT(1)
FROM Customers c
WHERE c.IsDeleted = 0
  AND c.ExternalId = @CustomerExternalId;";

        public const string SellerExistsByExternalId = @"
SELECT COUNT(1)
FROM Users u
WHERE u.IsActive = 1
  AND u.ExternalId = @SellerExternalId;";

        public const string GetUserInternalIdByExternalId = @"
SELECT TOP 1
    Id
FROM Users
WHERE IsActive = 1
  AND ExternalId = @ExternalId;";

        public const string Update = @"
UPDATE p
SET
    p.Name = @Name,
    p.Description = @Description,
    p.CustomerId = c.Id,
    p.SellerId = u.Id,
    p.EstimatedAmount = @EstimatedAmount,
    p.StartDateUtc = @StartDateUtc,
    p.ExpectedCloseDateUtc = @ExpectedCloseDateUtc,
    p.ProgressPercentage = @ProgressPercentage,
    p.ActualCloseDateUtc = @ActualCloseDateUtc,
    p.Address = @Address,
    p.Latitude = @Latitude,
    p.Longitude = @Longitude,
    p.UpdatedAtUtc = SYSUTCDATETIME()
FROM Projects p
INNER JOIN Customers c ON c.ExternalId = @CustomerExternalId AND c.IsDeleted = 0
INNER JOIN Users u ON u.ExternalId = @SellerExternalId AND u.IsActive = 1
WHERE p.IsDeleted = 0
  AND p.ExternalId = @ExternalId;";

        public const string ChangeStatus = @"
UPDATE Projects
SET StatusId = @StatusId,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE IsDeleted = 0
  AND ExternalId = @ExternalId;";

        public const string Delete = @"
UPDATE Projects
SET IsDeleted = 1,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE IsDeleted = 0
  AND ExternalId = @ExternalId;";
    }
}