namespace SalesTracking.Infrastructure.Persistence.Sql.Auth
{
    public static class InvitationRepositoryQueries
    {
        
        public const string GetUserIdByEmail = @"
                SELECT Id
                FROM Users
                WHERE Email = @Email
                  AND IsActive = 1;";       

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
