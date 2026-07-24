namespace SalesTracking.Infrastructure.Persistence.Sql.Customers
{
    public static class CustomerRepositoryQueries
    {
        public const string GetCustomerDetail = @"
DECLARE @CustomerId INT = (
    SELECT TOP 1 Id
    FROM Customers
    WHERE ExternalId = @ExternalId
      AND CompanyId = @CompanyId
      AND IsDeleted = 0
);
" + GetCustomerByExternalId + GetCustomerNotesByCustomerId + GetCustomerRemindersByCustomerId;

        public const string GetCustomersPage = GetCustomers + CountCustomers;

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
                                            AND c.CompanyId = @CompanyId
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
                                                AND c.CompanyId = @CompanyId
                                                AND (@StatusId IS NULL OR c.StatusId = @StatusId)
                                                AND (@SellerExternalId IS NULL OR u.ExternalId = @SellerExternalId)
                                                AND (
                                                    @Search IS NULL
                                                    OR c.Name LIKE @Search
                                                    OR c.CompanyName LIKE @Search
                                                    OR c.Email LIKE @Search
                                                );";

        public const string GetCustomerByExternalId = @"
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
                                                AND c.CompanyId = @CompanyId
                                                AND c.IsDeleted = 0;";

        public const string GetCustomerNotesByCustomerId = @"
                                                        SELECT
                                                            n.Id,                                                            
                                                            n.ExternalId,
                                                            n.Text,
                                                            u.Id AS AuthorId,
                                                            u.ExternalId AS AuthorExternalId,
                                                            u.FullName AS AuthorName,
                                                            n.CreatedAtUtc AS CreatedAt
                                                        FROM CustomerNotes n
                                                        INNER JOIN Users u ON n.AuthorId = u.Id
                                                        WHERE n.CustomerId = @CustomerId
                                                        AND n.CompanyId = @CompanyId
                                                        ORDER BY n.CreatedAtUtc DESC;";

        public const string GetCustomerRemindersByCustomerId = @"
                                                        SELECT
                                                            r.Id,
                                                            r.ExternalId,
                                                            r.Text,
                                                            r.ReminderAtUtc,
                                                            u.Id AS AssignedToId,
                                                            u.ExternalId AS AssignedToExternalId,
                                                            u.FullName AS AssignedToName,
                                                            r.Completed
                                                        FROM CustomerReminders r
                                                        INNER JOIN Users u ON u.Id = r.AssignedToId
                                                        WHERE r.CustomerId = @CustomerId
                                                        AND r.CompanyId = @CompanyId
                                                        ORDER BY r.Completed ASC, r.ReminderAtUtc ASC;";
        //CREATE:
        public const string GetSellerInternalIdByExternalId = @"
                                                            SELECT TOP 1
                                                                u.Id
                                                            FROM Users u
                                                            INNER JOIN UserRoles ur ON u.Id = ur.UserId
                                                            INNER JOIN Roles r ON ur.RoleId = r.Id
                                                            WHERE u.ExternalId = @ExternalId
                                                            AND u.CompanyId = @CompanyId
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
                                                    CompanyId,
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
                                                    @CompanyId,
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
                                CompanyId,
                                CreatedAtUtc
                            )
                            VALUES (
                                @ExternalId,
                                @CustomerId,
                                @EventType,
                                @Description,
                                @CreatedById,
                                @CompanyId,
                                SYSUTCDATETIME()
                            );";

        public const string GetCustomerInternalIdByExternalId = @"
                                    SELECT TOP 1
                                        Id
                                    FROM Customers
                                    WHERE ExternalId = @ExternalId
                                    AND CompanyId = @CompanyId
                                    AND IsDeleted = 0;";

        public const string UpdateCustomer = @"
                                    UPDATE Customers
                                    SET
                                        Name = @Name,
                                        CompanyName = @CompanyName,
                                        Phone = @Phone,
                                        Email = @Email,
                                        SellerId = COALESCE(@SellerId, SellerId),
                                        Address = @Address,
                                        Latitude = @Latitude,
                                        Longitude = @Longitude,
                                        UpdatedAtUtc = SYSUTCDATETIME()
                                    WHERE Id = @CustomerId
                                    AND CompanyId = @CompanyId
                                    AND IsDeleted = 0;";

        public const string ChangeCustomerStatus = @"
                                    UPDATE Customers
                                    SET
                                        StatusId = @StatusId,
                                        UpdatedAtUtc = SYSUTCDATETIME()
                                    WHERE Id = @CustomerId
                                    AND CompanyId = @CompanyId
                                    AND IsDeleted = 0;";

        public const string DeleteCustomer = @"
                                    UPDATE Customers
                                    SET
                                        IsDeleted = 1,
                                        UpdatedAtUtc = SYSUTCDATETIME()
                                    WHERE Id = @CustomerId
                                    AND CompanyId = @CompanyId
                                    AND IsDeleted = 0;";
    }
}
