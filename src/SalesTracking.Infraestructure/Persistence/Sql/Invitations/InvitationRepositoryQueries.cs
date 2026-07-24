namespace SalesTracking.Infrastructure.Persistence.Sql.Auth;

public static class InvitationRepositoryQueries
{
    public const string GetInvitationByToken = @"
SELECT TOP 1
    i.Email,
    i.FullName,
    r.Code AS RoleCode,
    i.InvitedBy,
    i.CompanyId,
    c.Name AS CompanyName,
    i.ExpiresAtUtc
FROM Invitations i
INNER JOIN Companies c ON c.Id = i.CompanyId
INNER JOIN Roles r ON r.Id = i.RoleId
WHERE i.TokenHash = @TokenHash
  AND i.AcceptedAtUtc IS NULL
  AND i.ExpiresAtUtc > SYSUTCDATETIME();";

    public const string GetAssignableRoleId = @"
SELECT TOP 1 r.Id
FROM Roles r
WHERE r.Code = @RoleCode
  AND NOT EXISTS (
      SELECT 1
      FROM RolePermissions rp
      INNER JOIN Permissions p ON p.Id = rp.PermissionId
      WHERE rp.RoleId = r.Id
        AND p.Code NOT IN @InviterPermissions
  );";

    public const string EmailExistsInCompany = @"
SELECT COUNT(1) FROM Users
WHERE CompanyId = @CompanyId AND Email = @Email;";

    public const string PendingInvitationExists = @"
SELECT COUNT(1) FROM Invitations
WHERE CompanyId = @CompanyId AND Email = @Email
  AND AcceptedAtUtc IS NULL
  AND ExpiresAtUtc > SYSUTCDATETIME();";

    public const string CreateInvitation = @"
INSERT INTO Invitations (
    TokenHash, Email, FullName, CompanyId, InvitedBy, RoleId,
    ExpiresAtUtc, CreatedAtUtc
)
VALUES (
    @TokenHash, @Email, @FullName, @CompanyId, @InvitedBy, @RoleId,
    @ExpiresAtUtc, SYSUTCDATETIME()
);";

    public const string GetInvitationForAcceptance = @"
SELECT TOP 1
    inv.Id, inv.Email, inv.FullName, inv.CompanyId, inv.RoleId,
    c.Name AS CompanyName, c.ExternalId AS CompanyExternalId,
    inv.ExpiresAtUtc, inv.AcceptedAtUtc
FROM Invitations inv WITH (UPDLOCK, ROWLOCK)
INNER JOIN Companies c ON c.Id = inv.CompanyId
WHERE inv.TokenHash = @TokenHash;";

    public const string InsertUser = @"
INSERT INTO Users (
    ExternalId, Username, Email, FullName, PasswordHash,
    CompanyId, IsActive, CreatedAtUtc
)
OUTPUT INSERTED.Id
VALUES (
    @ExternalId, @Username, @Email, @FullName, @PasswordHash,
    @CompanyId, 1, SYSUTCDATETIME()
);";

    public const string InsertUserRole = @"
INSERT INTO UserRoles (UserId, RoleId)
VALUES (@UserId, @RoleId);";

    public const string MarkInvitationAccepted = @"
UPDATE Invitations
SET AcceptedAtUtc = SYSUTCDATETIME()
WHERE Id = @InvitationId
  AND AcceptedAtUtc IS NULL
  AND ExpiresAtUtc > SYSUTCDATETIME();";
}
