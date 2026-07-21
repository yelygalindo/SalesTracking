namespace SalesTracking.Infrastructure.Persistence.Sql.Authorization;

internal static class UserAuthorizationQueries
{
    public const string Get = @"
SELECT DISTINCT r.Code
FROM UserRoles ur
INNER JOIN Roles r ON r.Id = ur.RoleId
INNER JOIN Users u ON u.Id = ur.UserId
WHERE ur.UserId = @UserId AND u.IsActive = 1;

SELECT DISTINCT p.Code
FROM UserRoles ur
INNER JOIN RolePermissions rp ON rp.RoleId = ur.RoleId
INNER JOIN Permissions p ON p.Id = rp.PermissionId
INNER JOIN Users u ON u.Id = ur.UserId
WHERE ur.UserId = @UserId AND u.IsActive = 1;";
}
