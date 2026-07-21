namespace SalesTracking.Application.UseCases.Dashboard.Results
{
    public sealed class DashboardResult
    {
        public DashboardMetricsResult Metrics { get; set; } = new DashboardMetricsResult();
        public IReadOnlyList<DashboardProjectLocationResult> ProjectLocations { get; set; } = new List<DashboardProjectLocationResult>();
        public IReadOnlyList<DashboardRecentActivityResult> RecentActivity { get; set; } = new List<DashboardRecentActivityResult>();
        public IReadOnlyList<DashboardUpcomingFollowUpResult> UpcomingFollowUps { get; set; } = new List<DashboardUpcomingFollowUpResult>();
        public IReadOnlyList<DashboardUrgentDeliveryResult> UrgentDeliveries { get; set; } = new List<DashboardUrgentDeliveryResult>();
    }
}
