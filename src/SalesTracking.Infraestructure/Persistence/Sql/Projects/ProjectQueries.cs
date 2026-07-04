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
    CreatedAtUtc
)
OUTPUT INSERTED.Id
SELECT
    p.Id,
    p.ExternalId,
    p.Name,
    p.Description,
    c.Id AS CustomerId,
    u.Id AS UserId,
    p.StatusId,
    p.EstimatedAmount,
    p.StartDateUtc,
    p.ExpectedCloseDateUtc,
    SYSUTCDATETIME() AS RetrievedAt
FROM Projects AS p
INNER JOIN Customers AS c
    ON c.Id = p.CustomerId
INNER JOIN Users AS u
    ON u.Id = p.SellerId
WHERE p.Id = @Id
  AND c.IsDeleted = 0
  AND u.IsActive = 1;";

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
    p.CreatedAtUtc
FROM Projects p
INNER JOIN Customers c ON c.Id = p.CustomerId
INNER JOIN Users u ON u.Id = p.SellerId
INNER JOIN ProjectStatus ps ON ps.ProjectStatusId = p.StatusId
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
