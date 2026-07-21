namespace SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Rows
{
    internal sealed class DashboardRecentActivityRow
    {
        public string ProjectExternalId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? UserExternalId { get; set; }
        public string? UserName { get; set; }
        public DateTime OccurredAtUtc { get; set; }
    }
}
