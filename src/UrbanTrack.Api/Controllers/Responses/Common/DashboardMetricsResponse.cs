using System;
using System.Collections.Generic;

namespace UrbanTrack.Api.Controllers.Responses.Dashboard
{
    public class DashboardMetricsResponse
    {
        public int Prospects { get; set; }
        public int ActiveCustomers { get; set; }
        public int ActiveProjects { get; set; }
        public int PendingDeliveries { get; set; }
        public decimal MonthlySales { get; set; }
        public int TodayFollowUps { get; set; }
        public int ActiveSellers { get; set; }
        public IEnumerable<string> RecentActivity { get; set; } = new List<string>();
        public IEnumerable<FollowUpItem> UpcomingFollowUps { get; set; } = new List<FollowUpItem>();
    }

    public class FollowUpItem
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime When { get; set; }
        public string Note { get; set; }
    }
}