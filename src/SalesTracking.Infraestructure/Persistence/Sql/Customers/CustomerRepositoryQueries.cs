namespace SalesTracking.Infrastructure.Persistence.Sql.Customers
{
    public static class CustomerRepositoryQueries
    {
        public const string GetCustomers = @"
                                            SELECT
                                                c.Id,
                                                c.ExternalId,
                                                c.Name,
                                                c.CompanyName,
                                                c.Phone,
                                                c.Email,
                                                c.StatusId,
                                                u.ExternalId AS ExternalSellerId,
                                                u.Id AS SellerId,
                                                u.UserName AS SellerUserName,
                                                c.CreatedAtUtc AS CreatedAt
                                            FROM Customers c
                                            LEFT JOIN Users u ON c.SellerId = u.Id
                                            WHERE c.IsDeleted = 0
                                            AND (@StatusId IS NULL OR c.StatusId = @StatusId)
                                            AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
                                            AND (
                                                @Search IS NULL
                                                OR c.Name LIKE @Search
                                                OR c.CompanyName LIKE @Search
                                                OR c.Email LIKE @Search
                                            )
                                            ORDER BY c.CreatedAtUtc DESC
                                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        public const string CountCustomers = @"
                                                SELECT COUNT(1)
                                                FROM Customers c
                                                LEFT JOIN Users u ON c.SellerId = u.Id
                                                WHERE c.IsDeleted = 0
                                                AND (@StatusId IS NULL OR c.StatusId = @StatusId)
                                                AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
                                                AND (
                                                    @Search IS NULL
                                                    OR c.Name LIKE @Search
                                                    OR c.CompanyName LIKE @Search
                                                    OR c.Email LIKE @Search
                                                );";

        public const string GetCustomerById = @"
                                                SELECT
                                                    c.Id,
                                                    c.ExternalId,
                                                    c.Name,
                                                    c.CompanyName,
                                                    c.Phone,
                                                    c.Email,
                                                    c.StatusId,
                                                    u.ExternalId AS ExternalSellerId,
                                                    u.Id AS SellerId,
                                                    u.UserName AS SellerUserName,
                                                    c.Address,
                                                    c.Latitude,
                                                    c.Longitude,
                                                    c.CreatedAtUtc AS CreatedAt
                                                FROM Customers c
                                                LEFT JOIN Users u ON c.SellerId = u.Id
                                                WHERE c.ExternalId = @ExternalId
                                                AND c.IsDeleted = 0;";

        public const string GetCustomerNotesByCustomerId = @"
                                                        SELECT
                                                            n.Id,                                                            
                                                            n.ExternalId,
                                                            n.Text,
                                                            u.ExternalId AS AuthorId,
                                                            n.CreatedAtUtc AS CreatedAt
                                                        FROM CustomerNotes n
                                                        INNER JOIN Users u ON n.AuthorId = u.Id
                                                        WHERE n.CustomerId = @CustomerId
                                                        ORDER BY n.CreatedAtUtc DESC;";
        //CREATE:
        public const string GetSellerInternalIdByExternalId = @"
                                                            SELECT TOP 1
                                                                u.Id
                                                            FROM Users u
                                                            INNER JOIN UserRoles ur ON u.Id = ur.UserId
                                                            INNER JOIN Roles r ON ur.RoleId = r.Id
                                                            WHERE u.ExternalId = @ExternalId
                                                            AND u.IsActive = 1";

        public const string CreateCustomer = @"
                                                INSERT INTO Customers (
                                                    ExternalId,
                                                    Name,
                                                    CompanyName,
                                                    Phone,
                                                    Email,
                                                    StatusId,
                                                    SellerId,
                                                    Address,
                                                    Latitude,
                                                    Longitude,
                                                    CreatedAtUtc,
                                                    IsDeleted
                                                )
                                                OUTPUT INSERTED.Id
                                                VALUES (
                                                    @ExternalId,
                                                    @Name,
                                                    @CompanyName,
                                                    @Phone,
                                                    @Email,
                                                    @StatusId,
                                                    @SellerId,
                                                    @Address,
                                                    @Latitude,
                                                    @Longitude,
                                                    SYSUTCDATETIME(),
                                                    0
                                                );";

        public const string CreateCustomerTimelineEvent = @"
                            INSERT INTO CustomerTimelineEvents (
                                ExternalId,
                                CustomerId,
                                EventType,
                                Description,
                                CreatedById,
                                CreatedAtUtc
                            )
                            VALUES (
                                @ExternalId,
                                @CustomerId,
                                @EventType,
                                @Description,
                                @CreatedById,
                                SYSUTCDATETIME()
                            );";
    }
}
