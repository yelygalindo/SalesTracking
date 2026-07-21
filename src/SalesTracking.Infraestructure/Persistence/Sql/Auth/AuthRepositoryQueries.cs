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
                    u.ExternalId,
                    rt.ExpiresAtUtc,
                    rt.RevokedAtUtc,
                    u.Id AS AuthUserId,
                    u.Username,
                    u.FullName,
                    u.Email,
                    c.Id AS CompanyId,
                    c.ExternalId AS CompanyExternalId,
                    c.Name AS CompanyName
                FROM RefreshTokens rt WITH (UPDLOCK, ROWLOCK)
                INNER JOIN Users u ON rt.UserId = u.Id
                LEFT JOIN Companies c ON u.CompanyId = c.Id
                WHERE rt.TokenHash = @TokenHash
                  AND rt.RevokedAtUtc IS NULL
                  AND rt.ExpiresAtUtc > SYSUTCDATETIME()
                  AND u.IsActive = 1;";

        public const string UpdateOldRefreshToken = @"
                UPDATE RefreshTokens
                SET 
                    RevokedAtUtc = SYSUTCDATETIME(),
                    ReplacedByTokenHash = @NewTokenHash
                WHERE Id = @RefreshTokenId
                  AND RevokedAtUtc IS NULL;";

        public const string RevokeUserRefreshTokens = @"
                UPDATE RefreshTokens
                SET RevokedAtUtc = SYSUTCDATETIME()
                WHERE UserId = @UserId
                  AND RevokedAtUtc IS NULL;";

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
                UPDATE PasswordResetTokens
                SET UsedAtUtc = SYSUTCDATETIME()
                WHERE UserId = @UserId
                  AND UsedAtUtc IS NULL;

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
                FROM PasswordResetTokens WITH (UPDLOCK, ROWLOCK)
                WHERE TokenHash = @TokenHash
                  AND UsedAtUtc IS NULL
                  AND ExpiresAtUtc > SYSUTCDATETIME();";

        public const string UpdateUserPassword = @"
                UPDATE Users
                SET PasswordHash = @PasswordHash
                WHERE Id = @UserId
                  AND IsActive = 1;";

        public const string MarkPasswordResetTokenUsed = @"
                UPDATE PasswordResetTokens
                SET UsedAtUtc = SYSUTCDATETIME()
                WHERE Id = @Id
                  AND UsedAtUtc IS NULL
                  AND ExpiresAtUtc > SYSUTCDATETIME();";

        public const string GetUserById = @"
                SELECT 
                    u.Id,
                    u.ExternalId,
                    u.Username,
                    u.FullName,
                    u.Email,
                    c.Id AS CompanyId,
                    c.ExternalId AS CompanyExternalId,
                    c.Name AS CompanyName
                FROM Users u
                LEFT JOIN Companies c ON u.CompanyId = c.Id
                WHERE u.Id = @UserId
                  AND u.IsActive = 1;";       

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
    }
}
