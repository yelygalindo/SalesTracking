using SalesTracking.Application.UseCases.Dashboard.Results;
using SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Mappers
{
    internal static class DashboardRepositoryMapper
    {
        public static DashboardMetricsResult ToResult(this DashboardMetricsRow row)
        {
            return new DashboardMetricsResult
            {
                Prospects = row.Prospects,
                ActiveCustomers = row.ActiveCustomers,
                ActiveProjects = row.ActiveProjects,
                PendingDeliveries = row.PendingDeliveries,
                OverdueDeliveries = row.OverdueDeliveries,
                TodayFollowUps = row.TodayFollowUps,
                CompletedDeliveriesThisMonth = row.CompletedDeliveriesThisMonth
            };
        }

        public static DashboardProjectLocationResult ToResult(this DashboardProjectLocationRow row) => new DashboardProjectLocationResult
        {
            ProjectExternalId = row.ProjectExternalId,
            Name = row.Name,
            StatusId = row.StatusId,
            StatusName = row.StatusName,
            CustomerName = row.CustomerName,
            SellerName = row.SellerName,
            ProgressPercentage = row.ProgressPercentage,
            Address = row.Address,
            Latitude = row.Latitude,
            Longitude = row.Longitude
        };

        public static DashboardRecentActivityResult ToResult(this DashboardRecentActivityRow row) => new DashboardRecentActivityResult
        {
            ProjectExternalId = row.ProjectExternalId,
            ProjectName = row.ProjectName,
            EventTypeId = row.EventTypeId,
            EventTypeName = row.EventTypeName,
            Title = row.Title,
            UserExternalId = row.UserExternalId,
            UserName = row.UserName,
            OccurredAtUtc = row.OccurredAtUtc
        };

        public static DashboardUpcomingFollowUpResult ToResult(this DashboardUpcomingFollowUpRow row) => new DashboardUpcomingFollowUpResult
        {
            ReminderExternalId = row.ReminderExternalId,
            CustomerExternalId = row.CustomerExternalId,
            CustomerName = row.CustomerName,
            Text = row.Text,
            ReminderAtUtc = row.ReminderAtUtc,
            AssignedToExternalId = row.AssignedToExternalId,
            AssignedToName = row.AssignedToName
        };

        public static DashboardUrgentDeliveryResult ToResult(this DashboardUrgentDeliveryRow row) => new DashboardUrgentDeliveryResult
        {
            DeliveryExternalId = row.DeliveryExternalId,
            ProjectExternalId = row.ProjectExternalId,
            ProjectName = row.ProjectName,
            StatusId = row.StatusId,
            StatusName = row.StatusName,
            CommittedDateUtc = row.CommittedDateUtc,
            DeliveredDateUtc = row.DeliveredDateUtc,
            IsOverdue = row.IsOverdue
        };
    }
}
