namespace SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Rows
{
    internal sealed class DashboardUpcomingFollowUpRow
    {
        public string ReminderExternalId { get; set; } = string.Empty;
        public string CustomerExternalId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime ReminderAtUtc { get; set; }
        public string AssignedToExternalId { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = string.Empty;
    }
}
