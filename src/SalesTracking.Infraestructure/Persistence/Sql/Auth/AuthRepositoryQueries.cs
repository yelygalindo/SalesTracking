namespace SalesTracking.Infrastructure.Persistence.Sql.Auth
{
    public static class AuthRepositoryQueries
    {
        public const string RevokeRefreshToken = @"
                UPDATE RefreshTokens
                SET RevokedAtUtc = SYSUTCDATETIME()
                WHERE TokenHash = @TokenHash 
                  AND RevokedAtUtc IS NULL;";

        public const string SelectRefreshTokenWithUser = @"
                SELECT 
                    rt.Id AS RefreshTokenId,
                    rt.UserId,
                    rt.ExpiresAtUtc,
                    rt.RevokedAtUtc,
                    u.Id AS AuthUserId,
                    u.Username,
                    u.FullName,
                    u.Email,
                    c.Id AS CompanyId,
                    c.ExternalId AS CompanyExternalId,
                    c.Name AS CompanyName
                FROM RefreshTokens rt
                INNER JOIN Users u ON rt.UserId = u.Id
                LEFT JOIN Companies c ON u.CompanyId = c.Id
                WHERE rt.TokenHash = @TokenHash
                  AND u.IsActive = 1;";

        public const string UpdateOldRefreshToken = @"
                UPDATE RefreshTokens
                SET 
                    RevokedAtUtc = SYSUTCDATETIME(),
                    ReplacedByTokenHash = @NewTokenHash
                WHERE Id = @RefreshTokenId;";

        public const string InsertNewRefreshToken = @"
                INSERT INTO RefreshTokens (
                    UserId,
                    TokenHash,
                    ExpiresAtUtc,
                    CreatedAtUtc
                )
                VALUES (
                    @UserId,
                    @TokenHash,
                    @ExpiresAtUtc,
                    SYSUTCDATETIME()
                );";

        public const string GetUserIdByEmail = @"
                SELECT Id
                FROM Users
                WHERE Email = @Email
                  AND IsActive = 1;";

        public const string InsertPasswordResetToken = @"
                INSERT INTO PasswordResetTokens (
                    UserId,
                    TokenHash,
                    ExpiresAtUtc,
                    CreatedAtUtc
                )
                VALUES (
                    @UserId,
                    @TokenHash,
                    @ExpiresAtUtc,
                    SYSUTCDATETIME()
                );";

        public const string SelectPasswordResetToken = @"
                SELECT 
                    Id,
                    UserId,
                    ExpiresAtUtc,
                    UsedAtUtc
                FROM PasswordResetTokens
                WHERE TokenHash = @TokenHash;";

        public const string UpdateUserPassword = @"
                UPDATE Users
                SET PasswordHash = @PasswordHash
                WHERE Id = @UserId
                  AND IsActive = 1;";

        public const string MarkPasswordResetTokenUsed = @"
                UPDATE PasswordResetTokens
                SET UsedAtUtc = SYSUTCDATETIME()
                WHERE Id = @Id;";

        public const string GetUserById = @"
                SELECT 
                    u.Id,
                    u.Username,
                    u.FullName,
                    c.Id AS CompanyId,
                    c.Name AS CompanyName
                FROM Users u
                LEFT JOIN Companies c ON u.CompanyId = c.Id
                WHERE u.Id = @UserId
                  AND u.IsActive = 1;";

        public const string GetInvitationByToken = @"
                SELECT 
                    i.TokenHash AS Token,
                    i.Email,
                    i.InvitedBy,
                    i.CompanyId,
                    c.Name AS CompanyName,
                    i.ExpiresAtUtc
                FROM Invitations i
                INNER JOIN Companies c ON i.CompanyId = c.Id
                WHERE i.TokenHash = @TokenHash
                  AND i.AcceptedAtUtc IS NULL
                  AND i.ExpiresAtUtc > SYSUTCDATETIME();";

        public const string GetInvitation = @"
                SELECT 
                    inv.Id,
                    inv.Email,
                    c.Id AS CompanyId,
                    c.Name AS CompanyName,
                    c.ExternalId AS CompanyExternalId,                    
                    inv.ExpiresAtUtc,
                    inv.AcceptedAtUtc
                FROM Invitations as inv Inner JOIN Companies c ON c.Id = inv.CompanyId
                WHERE TokenHash = @TokenHash;";

        public const string InsertUser = @"
                INSERT INTO Users (
                    ExternalId,
                    Username,
                    Email,
                    FullName,
                    PasswordHash,
                    CompanyId,
                    IsActive,
                    CreatedAtUtc
                )
                OUTPUT INSERTED.Id
                VALUES (
                    @ExternalId,
                    @Username,
                    @Email,
                    @FullName,
                    @PasswordHash,
                    @CompanyId,
                    1,
                    SYSUTCDATETIME()
                );";

        public const string MarkInvitationAccepted = @"
                UPDATE Invitations
                SET AcceptedAtUtc = SYSUTCDATETIME()
                WHERE Id = @InvitationId;";

        public const string SelectUserForEmail = @"
                SELECT 
                    u.Id,
                    u.ExternalId,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.PasswordHash,
                    c.Id AS CompanyId,
                    c.Name AS CompanyName,
                    c.ExternalId AS CompanyExternalId,
                    u.IsActive
                FROM Users u 
                LEFT JOIN Companies c ON u.CompanyId = c.Id
                WHERE u.Email = @Email 
                  AND u.IsActive = 1;";

        public const string InsertRefreshToken = @"
                INSERT INTO RefreshTokens (
                    UserId,
                    TokenHash,
                    ExpiresAtUtc,
                    CreatedAtUtc
                )
                VALUES (
                    @UserId,
                    @TokenHash,
                    @ExpiresAtUtc,
                    SYSUTCDATETIME()
                );";

        public const string CreateInvitation = @"
                INSERT INTO Invitations (
                    TokenHash,
                    Email,
                    CompanyId,
                    InvitedBy,
                    ExpiresAtUtc,
                    CreatedAtUtc
                )
                VALUES (
                    @TokenHash,
                    @Email,
                    @CompanyId,
                    @InvitedBy,
                    @ExpiresAtUtc,
                    SYSUTCDATETIME()
                );

                SELECT 
                    i.TokenHash,
                    i.Email,
                    i.InvitedBy,
                    i.CompanyId,
                    c.Name AS CompanyName,
                    i.ExpiresAtUtc
                FROM Invitations i
                INNER JOIN Companies c ON i.CompanyId = c.Id
                WHERE i.TokenHash = @TokenHash;";
    }
}
