using SalesTracking.Application.UseCases.Reports.Results;
using SalesTracking.Infrastructure.Persistence.Sql.Reports.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.Reports.Mappers
{
    internal static class ReportRepositoryMapper
    {
        public static DeliveryReportResult ToResult(this DeliveryReportRow row)
        {
            return new DeliveryReportResult
            {
                DeliveryExternalId = row.DeliveryExternalId,
                ProjectExternalId = row.ProjectExternalId,
                ProjectName = row.ProjectName,
                CustomerExternalId = row.CustomerExternalId,
                CustomerName = row.CustomerName,
                SellerExternalId = row.SellerExternalId,
                SellerName = row.SellerName,
                StatusId = row.StatusId,
                StatusName = row.StatusName,
                CommittedDateUtc = row.CommittedDateUtc,
                DeliveredDateUtc = row.DeliveredDateUtc,
                TotalQuantity = row.TotalQuantity,
                DeliveredQuantity = row.DeliveredQuantity,
                Notes = row.Notes,
                CreatedAtUtc = row.CreatedAtUtc,
                UpdatedAtUtc = row.UpdatedAtUtc
            };
        }

        public static CustomerPendingContactReportResult ToResult(this CustomerPendingContactReportRow row)
        {
            return new CustomerPendingContactReportResult
            {
                ReminderExternalId = row.ReminderExternalId,
                CustomerExternalId = row.CustomerExternalId,
                CustomerName = row.CustomerName,
                CompanyName = row.CompanyName,
                Phone = row.Phone,
                Email = row.Email,
                StatusId = row.StatusId,
                SellerExternalId = row.SellerExternalId,
                SellerName = row.SellerName,
                Text = row.Text,
                ReminderAtUtc = row.ReminderAtUtc,
                AssignedToExternalId = row.AssignedToExternalId,
                AssignedToName = row.AssignedToName
            };
        }

        public static ProjectReportResult ToResult(this ProjectReportRow row)
        {
            return new ProjectReportResult
            {
                ProjectExternalId = row.ProjectExternalId,
                Name = row.Name,
                Description = row.Description,
                CustomerExternalId = row.CustomerExternalId,
                CustomerName = row.CustomerName,
                SellerExternalId = row.SellerExternalId,
                SellerName = row.SellerName,
                StatusId = row.StatusId,
                StatusName = row.StatusName,
                EstimatedAmount = row.EstimatedAmount,
                StartDateUtc = row.StartDateUtc,
                ExpectedCloseDateUtc = row.ExpectedCloseDateUtc,
                ActualCloseDateUtc = row.ActualCloseDateUtc,
                ProgressPercentage = row.ProgressPercentage,
                Address = row.Address,
                Latitude = row.Latitude,
                Longitude = row.Longitude,
                CreatedAtUtc = row.CreatedAtUtc,
                UpdatedAtUtc = row.UpdatedAtUtc
            };
        }

        public static CommercialActivityReportResult ToResult(this CommercialActivityReportRow row)
        {
            return new CommercialActivityReportResult
            {
                TimelineExternalId = row.TimelineExternalId,
                ProjectExternalId = row.ProjectExternalId,
                ProjectName = row.ProjectName,
                ProjectStatusId = row.ProjectStatusId,
                ProjectStatusName = row.ProjectStatusName,
                SellerExternalId = row.SellerExternalId,
                SellerName = row.SellerName,
                EventTypeId = row.EventTypeId,
                EventTypeName = row.EventTypeName,
                Title = row.Title,
                Description = row.Description,
                OccurredAtUtc = row.OccurredAtUtc,
                CreatedByExternalId = row.CreatedByExternalId,
                CreatedByName = row.CreatedByName,
                RelatedEntityType = row.RelatedEntityType,
                RelatedEntityId = row.RelatedEntityId,
                MetadataJson = row.MetadataJson
            };
        }
    }
}
