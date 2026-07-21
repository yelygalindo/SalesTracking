namespace UrbanTrack.Api.Controllers.Responses.Dashboard
{
    public sealed class DashboardResponse
    {
        public DashboardMetricsResponse Metrics { get; set; } = new DashboardMetricsResponse();
        public IReadOnlyList<DashboardProjectLocationResponse> ProjectLocations { get; set; } = new List<DashboardProjectLocationResponse>();
        public IReadOnlyList<DashboardRecentActivityResponse> RecentActivity { get; set; } = new List<DashboardRecentActivityResponse>();
        public IReadOnlyList<DashboardUpcomingFollowUpResponse> UpcomingFollowUps { get; set; } = new List<DashboardUpcomingFollowUpResponse>();
        public IReadOnlyList<DashboardUrgentDeliveryResponse> UrgentDeliveries { get; set; } = new List<DashboardUrgentDeliveryResponse>();
    }

    public sealed class DashboardMetricsResponse
    {
        public int Prospects { get; set; }
        public int ActiveCustomers { get; set; }
        public int ActiveProjects { get; set; }
        public int PendingDeliveries { get; set; }
        public int OverdueDeliveries { get; set; }
        public int TodayFollowUps { get; set; }
        public int CompletedDeliveriesThisMonth { get; set; }
    }

    public sealed class DashboardProjectLocationResponse
    {
        public string ProjectExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal ProgressPercentage { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public sealed class DashboardRecentActivityResponse
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

    public sealed class DashboardUpcomingFollowUpResponse
    {
        public string ReminderExternalId { get; set; } = string.Empty;
        public string CustomerExternalId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime ReminderAtUtc { get; set; }
        public string AssignedToExternalId { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = string.Empty;
    }

    public sealed class DashboardUrgentDeliveryResponse
    {
        public string DeliveryExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public bool IsOverdue { get; set; }
    }
}
