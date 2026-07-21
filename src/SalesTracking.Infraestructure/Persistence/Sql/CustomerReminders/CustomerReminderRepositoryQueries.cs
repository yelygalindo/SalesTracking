namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders
{
    public static class CustomerReminderRepositoryQueries
    {
        public const string GetCustomerInternalIdByExternalId = @"
                                                SELECT TOP 1
                                                    Id
                                                FROM Customers
                                                WHERE ExternalId = @ExternalId
                                                AND CompanyId = @CompanyId
                                                AND IsDeleted = 0;";

        public const string GetUserInternalIdByExternalId = @"
                                                SELECT TOP 1
                                                    Id
                                                FROM Users
        WHERE ExternalId = @ExternalId
          AND CompanyId = @CompanyId
          AND IsActive = 1;";

        public const string GetRemindersByCustomerExternalId = @"
                                                SELECT
                                                    r.Id,
                                                    r.ExternalId AS ExternalId,
                                                    r.Text,
                                                    r.ReminderAtUtc,
                                                    u.Id AS AssignedToId,
                                                    u.ExternalId AS AssignedToExternalId,
                                                    u.UserName AS AssignedToName,
                                                    r.Completed
                                                FROM CustomerReminders r
                                                INNER JOIN Customers c ON c.Id = r.CustomerId
                                                INNER JOIN Users u ON  u.Id = r.AssignedToId
                                                WHERE c.ExternalId = @CustomerExternalId
                                                AND c.CompanyId = @CompanyId
                                                AND r.CompanyId = @CompanyId
                                                AND c.IsDeleted = 0
                                                ORDER BY r.Completed ASC, r.ReminderAtUtc ASC;";

        public const string CreateReminder = @"
                                                INSERT INTO CustomerReminders (
                                                    ExternalId,
                                                    CustomerId,
                                                    Text,
                                                    ReminderAtUtc,
                                                    AssignedToId,
                                                    CompanyId,
                                                    Completed,
                                                    CreatedAtUtc
                                                )
                                                VALUES (
                                                    @ExternalId,
                                                    @CustomerId,
                                                    @Text,
                                                    @ReminderAtUtc,
                                                    @AssignedToId,
                                                    @CompanyId,
                                                    0,
                                                    SYSUTCDATETIME()
                                                );";

        public const string CompleteReminder = @"
                                                UPDATE r
                                                SET
                                                    r.Completed = 1,
                                                    r.CompletedAtUtc = SYSUTCDATETIME()
                                                FROM CustomerReminders r
                                                INNER JOIN Customers c ON r.CustomerId = c.Id
                                                WHERE r.ExternalId = @ReminderExternalId
                                                AND c.ExternalId = @CustomerExternalId
                                                AND c.CompanyId = @CompanyId
                                                AND r.CompanyId = @CompanyId
                                                AND c.IsDeleted = 0
                                                AND r.Completed = 0;";

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
    }
}
