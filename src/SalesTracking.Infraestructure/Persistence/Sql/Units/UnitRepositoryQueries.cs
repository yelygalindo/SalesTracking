namespace SalesTracking.Infrastructure.Persistence.Sql.Units
{
    internal static class UnitRepositoryQueries
    {
        public const string Get = @"
SELECT
    u.Id,
    u.ExternalId,
    u.Name,
    u.Symbol,
    u.Description,
    u.AllowsDecimals,
    u.IsActive,
    u.CreatedAtUtc,
    u.UpdatedAtUtc,
    COUNT(1) OVER() AS TotalCount
FROM Units u
WHERE u.IsDeleted = 0
  AND u.CompanyId = @CompanyId
  AND (
      @Search IS NULL
      OR u.Name LIKE '%' + @Search + '%'
      OR u.Symbol LIKE '%' + @Search + '%'
      OR u.Description LIKE '%' + @Search + '%'
  )
ORDER BY u.CreatedAtUtc DESC
OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;";

        public const string GetByExternalId = @"
SELECT TOP 1
    u.Id,
    u.ExternalId,
    u.Name,
    u.Symbol,
    u.Description,
    u.AllowsDecimals,
    u.IsActive,
    u.CreatedAtUtc,
    u.UpdatedAtUtc
FROM Units u
WHERE u.IsDeleted = 0
  AND u.CompanyId = @CompanyId
  AND u.ExternalId = @ExternalId;";

        public const string Create = @"
INSERT INTO Units (
    ExternalId,
    Name,
    Symbol,
    Description,
    AllowsDecimals,
    IsActive,
    CreatedAtUtc,
    IsDeleted
    ,CompanyId
)
OUTPUT INSERTED.ExternalId
VALUES (
    @ExternalId,
    @Name,
    @Symbol,
    @Description,
    @AllowsDecimals,
    @IsActive,
    SYSUTCDATETIME(),
    0,
    @CompanyId
);";

        public const string Update = @"
UPDATE Units
SET
    Name = @Name,
    Symbol = @Symbol,
    Description = @Description,
    AllowsDecimals = @AllowsDecimals,
    IsActive = @IsActive,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE IsDeleted = 0
  AND CompanyId = @CompanyId
  AND ExternalId = @ExternalId;";

        public const string Delete = @"
UPDATE Units
SET
    IsDeleted = 1,
    UpdatedAtUtc = SYSUTCDATETIME()
WHERE IsDeleted = 0
  AND CompanyId = @CompanyId
  AND ExternalId = @ExternalId;";
    }
}
