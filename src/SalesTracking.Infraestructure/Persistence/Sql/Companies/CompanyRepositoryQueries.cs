namespace SalesTracking.Infrastructure.Persistence.Sql.Companies;

internal static class CompanyRepositoryQueries
{
    public const string CompanyNameExists = @"
SELECT COUNT(1)
FROM Companies
WHERE Name = @CompanyName;";

    public const string EmailExists = @"
SELECT COUNT(1)
FROM Users
WHERE Email = @AdminEmail;";

    public const string GetAdminRoleId = @"
SELECT TOP 1 Id
FROM Roles
WHERE Code = 'admin';";

    public const string InsertCompany = @"
INSERT INTO Companies (ExternalId, Name, CreatedAtUtc)
OUTPUT INSERTED.Id
VALUES (@CompanyExternalId, @CompanyName, SYSUTCDATETIME());";

    public const string InsertAdminUser = @"
INSERT INTO Users (
    ExternalId, Username, Email, FullName, PasswordHash,
    CompanyId, IsActive, CreatedAtUtc
)
OUTPUT INSERTED.Id
VALUES (
    @AdminUserExternalId, @AdminUsername, @AdminEmail, @AdminFullName, @PasswordHash,
    @CompanyId, 1, SYSUTCDATETIME()
);";

    public const string InsertUserRole = @"
INSERT INTO UserRoles (UserId, RoleId)
VALUES (@UserId, @RoleId);";
}
