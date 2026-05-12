namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes
{
    public static class CustomerNoteRepositoryQueries
    {
        public const string GetCustomerInternalIdByExternalId = @"
                            SELECT TOP 1
                                Id
                            FROM Customers
                            WHERE ExternalId = @ExternalId
                            AND IsDeleted = 0;";

        public const string GetUserInternalIdByExternalId = @"
                            SELECT TOP 1
                                Id
                            FROM Users
                            WHERE ExternalId = @ExternalId
                            AND IsActive = 1;";

        public const string GetNotesByCustomerExternalId = @"
                                SELECT
                                    n.Id,                                    
                                    n.ExternalId AS ExternalId,
                                    n.Text,
                                    u.Id AS AuthorId,
                                    u.ExternalId AS AuthorExternalId,
                                    u.Username AS AuthorName,
                                    n.CreatedAtUtc AS CreatedAt
                                FROM CustomerNotes n
                                INNER JOIN Customers c ON n.CustomerId = c.Id
                                INNER JOIN Users u ON n.AuthorId = u.Id
                                WHERE c.ExternalId = @CustomerExternalId
                                AND c.IsDeleted = 0
                                ORDER BY n.CreatedAtUtc DESC;";

        public const string AddNote = @"
                                INSERT INTO CustomerNotes (
                                    ExternalId,
                                    CustomerId,
                                    Text,
                                    AuthorId,
                                    CreatedAtUtc
                                )
                                VALUES (
                                    @ExternalId,
                                    @CustomerId,
                                    @Text,
                                    @AuthorId,
                                    SYSUTCDATETIME()
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
