namespace SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Rows
{
    internal sealed class DashboardMetricsRow
    {
        public int Prospects { get; set; }
        public int ActiveCustomers { get; set; }
        public int ActiveProjects { get; set; }
        public int PendingDeliveries { get; set; }
        public int OverdueDeliveries { get; set; }
        public int TodayFollowUps { get; set; }
        public int CompletedDeliveriesThisMonth { get; set; }
    }
}
