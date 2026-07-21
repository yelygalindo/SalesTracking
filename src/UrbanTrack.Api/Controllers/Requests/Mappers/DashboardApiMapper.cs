using SalesTracking.Application.UseCases.Dashboard.Comands;
using SalesTracking.Application.UseCases.Dashboard.Results;
using UrbanTrack.Api.Controllers.Requests.Dashboard;
using UrbanTrack.Api.Controllers.Responses.Dashboard;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class DashboardApiMapper
    {
        public static GetDashboardCommand ToApplication(this GetDashboardRequest request)
        {
            return new GetDashboardCommand(request.SellerExternalId, request.StatusId);
        }

        public static DashboardResponse ToResponse(this DashboardResult result)
        {
            return new DashboardResponse
            {
                Metrics = result.Metrics.ToResponse(),
                ProjectLocations = result.ProjectLocations.Select(x => x.ToResponse()).ToList(),
                RecentActivity = result.RecentActivity.Select(x => x.ToResponse()).ToList(),
                UpcomingFollowUps = result.UpcomingFollowUps.Select(x => x.ToResponse()).ToList(),
                UrgentDeliveries = result.UrgentDeliveries.Select(x => x.ToResponse()).ToList()
            };
        }

        private static DashboardMetricsResponse ToResponse(this DashboardMetricsResult result) => new DashboardMetricsResponse
        {
            Prospects = result.Prospects,
            ActiveCustomers = result.ActiveCustomers,
            ActiveProjects = result.ActiveProjects,
            PendingDeliveries = result.PendingDeliveries,
            OverdueDeliveries = result.OverdueDeliveries,
            TodayFollowUps = result.TodayFollowUps,
            CompletedDeliveriesThisMonth = result.CompletedDeliveriesThisMonth
        };

        private static DashboardProjectLocationResponse ToResponse(this DashboardProjectLocationResult result) => new DashboardProjectLocationResponse
        {
            ProjectExternalId = result.ProjectExternalId,
            Name = result.Name,
            StatusId = result.StatusId,
            StatusName = result.StatusName,
            CustomerName = result.CustomerName,
            SellerName = result.SellerName,
            ProgressPercentage = result.ProgressPercentage,
            Address = result.Address,
            Latitude = result.Latitude,
            Longitude = result.Longitude
        };

        private static DashboardRecentActivityResponse ToResponse(this DashboardRecentActivityResult result) => new DashboardRecentActivityResponse
        {
            ProjectExternalId = result.ProjectExternalId,
            ProjectName = result.ProjectName,
            EventTypeId = result.EventTypeId,
            EventTypeName = result.EventTypeName,
            Title = result.Title,
            UserExternalId = result.UserExternalId,
            UserName = result.UserName,
            OccurredAtUtc = result.OccurredAtUtc
        };

        private static DashboardUpcomingFollowUpResponse ToResponse(this DashboardUpcomingFollowUpResult result) => new DashboardUpcomingFollowUpResponse
        {
            ReminderExternalId = result.ReminderExternalId,
            CustomerExternalId = result.CustomerExternalId,
            CustomerName = result.CustomerName,
            Text = result.Text,
            ReminderAtUtc = result.ReminderAtUtc,
            AssignedToExternalId = result.AssignedToExternalId,
            AssignedToName = result.AssignedToName
        };

        private static DashboardUrgentDeliveryResponse ToResponse(this DashboardUrgentDeliveryResult result) => new DashboardUrgentDeliveryResponse
        {
            DeliveryExternalId = result.DeliveryExternalId,
            ProjectExternalId = result.ProjectExternalId,
            ProjectName = result.ProjectName,
            StatusId = result.StatusId,
            StatusName = result.StatusName,
            CommittedDateUtc = result.CommittedDateUtc,
            DeliveredDateUtc = result.DeliveredDateUtc,
            IsOverdue = result.IsOverdue
        };
    }
}
