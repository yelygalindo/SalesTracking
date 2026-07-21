namespace SalesTracking.Infrastructure.Persistence.Sql.Sellers;

internal static class SellerQueries
{
    public const string GetActive = @"
SELECT
    u.ExternalId,
    COALESCE(NULLIF(u.FullName, ''), u.Username, u.Email) AS DisplayName,
    u.Email
FROM Users u
WHERE u.CompanyId = @CompanyId
  AND u.IsActive = 1
ORDER BY COALESCE(NULLIF(u.FullName, ''), u.Username, u.Email);";
}
