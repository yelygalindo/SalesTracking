namespace SalesTracking.Infrastructure.Persistence.Sql.Products
{
    internal static class ProductRepositoryQueries
    {
        public const string Get = @"
SELECT
    p.Id,
    p.ExternalId,
    p.Code,
    p.Name,
    p.Description,
    p.UnitId,
    p.Price,
    p.IsActive,
    p.CreatedAtUtc,
    p.UpdatedAtUtc,
    COUNT(1) OVER() AS TotalCount
FROM Products p
WHERE p.IsDeleted = 0
  AND (
      @Search IS NULL
      OR p.Code LIKE '%' + @Search + '%'
      OR p.Name LIKE '%' + @Search + '%'
      OR p.Description LIKE '%' + @Search + '%'
  )
ORDER BY p.CreatedAtUtc DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;";

        public const string GetByExternalId = @"
SELECT TOP 1
    p.Id,
    p.ExternalId,
    p.Code,
    p.Name,
    p.Description,
    p.UnitId,
    p.Price,
    p.IsActive,
    p.CreatedAtUtc,
    p.UpdatedAtUtc
FROM Products p
WHERE p.IsDeleted = 0
  AND p.ExternalId = @ExternalId;";

        public const string Create = @"
INSERT INTO Products (
    ExternalId,
    Code,
    Name,
    Description,
    UnitId,
    Price,
    IsActive,
    CreatedAtUtc,
    IsDeleted
)
OUTPUT INSERTED.ExternalId
VALUES (
    @ExternalId,
    @Code,
    @Name,
    @Description,
    @UnitId,
    @Price,
    @IsActive,
    SYSUTCDATETIME(),
    0
);";

        public const string Update = @"
UPDATE Products
SET
    Code = @Code,
    Name = @Name,
    Description = @Description,
    UnitId = @UnitId,
    Price = @Price,
    IsActive = @IsActive,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE IsDeleted = 0
  AND ExternalId = @ExternalId;";

        public const string Delete = @"
UPDATE Products
SET
    IsDeleted = 1,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE IsDeleted = 0
  AND ExternalId = @ExternalId;";
    }
}