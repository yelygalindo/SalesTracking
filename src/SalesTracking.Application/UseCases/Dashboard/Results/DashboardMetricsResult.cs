namespace SalesTracking.Application.UseCases.Dashboard.Results
{
    public sealed class DashboardMetricsResult
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
