using SalesTracking.Application.UseCases.Reports.Comands;
using SalesTracking.Application.UseCases.Reports.Models;
using SalesTracking.Application.UseCases.Reports.Results;
using UrbanTrack.Api.Controllers.Requests.Reports;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Reports;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class ReportApiMapper
    {
        public static GetReportCommand ToApplication(this GetReportRequest request)
        {
            return new GetReportCommand(
                request.From,
                request.To,
                request.SellerExternalId,
                request.StatusId,
                request.Page,
                request.PageSize);
        }

        public static PagedResponse<DeliveryReportResponse> ToDeliveryResponse(
            this ReportPagedList<DeliveryReportResult> result)
        {
            return new PagedResponse<DeliveryReportResponse>
            {
                Items = result.Items.Select(x => x.ToResponse()).ToList(),
                Pagination = result.ToPaginationResponse()
            };
        }

        public static PagedResponse<CustomerPendingContactReportResponse> ToCustomerPendingContactResponse(
            this ReportPagedList<CustomerPendingContactReportResult> result)
        {
            return new PagedResponse<CustomerPendingContactReportResponse>
            {
                Items = result.Items.Select(x => x.ToResponse()).ToList(),
                Pagination = result.ToPaginationResponse()
            };
        }

        public static PagedResponse<ProjectReportResponse> ToProjectResponse(
            this ReportPagedList<ProjectReportResult> result)
        {
            return new PagedResponse<ProjectReportResponse>
            {
                Items = result.Items.Select(x => x.ToResponse()).ToList(),
                Pagination = result.ToPaginationResponse()
            };
        }

        public static PagedResponse<CommercialActivityReportResponse> ToCommercialActivityResponse(
            this ReportPagedList<CommercialActivityReportResult> result)
        {
            return new PagedResponse<CommercialActivityReportResponse>
            {
                Items = result.Items.Select(x => x.ToResponse()).ToList(),
                Pagination = result.ToPaginationResponse()
            };
        }

        private static DeliveryReportResponse ToResponse(this DeliveryReportResult result)
        {
            return new DeliveryReportResponse
            {
                DeliveryExternalId = result.DeliveryExternalId,
                ProjectExternalId = result.ProjectExternalId,
                ProjectName = result.ProjectName,
                CustomerExternalId = result.CustomerExternalId,
                CustomerName = result.CustomerName,
                SellerExternalId = result.SellerExternalId,
                SellerName = result.SellerName,
                StatusId = result.StatusId,
                StatusName = result.StatusName,
                CommittedDateUtc = result.CommittedDateUtc,
                DeliveredDateUtc = result.DeliveredDateUtc,
                TotalQuantity = result.TotalQuantity,
                DeliveredQuantity = result.DeliveredQuantity,
                Notes = result.Notes,
                CreatedAtUtc = result.CreatedAtUtc,
                UpdatedAtUtc = result.UpdatedAtUtc
            };
        }

        private static CustomerPendingContactReportResponse ToResponse(
            this CustomerPendingContactReportResult result)
        {
            return new CustomerPendingContactReportResponse
            {
                ReminderExternalId = result.ReminderExternalId,
                CustomerExternalId = result.CustomerExternalId,
                CustomerName = result.CustomerName,
                CompanyName = result.CompanyName,
                Phone = result.Phone,
                Email = result.Email,
                StatusId = result.StatusId,
                SellerExternalId = result.SellerExternalId,
                SellerName = result.SellerName,
                Text = result.Text,
                ReminderAtUtc = result.ReminderAtUtc,
                AssignedToExternalId = result.AssignedToExternalId,
                AssignedToName = result.AssignedToName
            };
        }

        private static ProjectReportResponse ToResponse(this ProjectReportResult result)
        {
            return new ProjectReportResponse
            {
                ProjectExternalId = result.ProjectExternalId,
                Name = result.Name,
                Description = result.Description,
                CustomerExternalId = result.CustomerExternalId,
                CustomerName = result.CustomerName,
                SellerExternalId = result.SellerExternalId,
                SellerName = result.SellerName,
                StatusId = result.StatusId,
                StatusName = result.StatusName,
                EstimatedAmount = result.EstimatedAmount,
                StartDateUtc = result.StartDateUtc,
                ExpectedCloseDateUtc = result.ExpectedCloseDateUtc,
                ActualCloseDateUtc = result.ActualCloseDateUtc,
                ProgressPercentage = result.ProgressPercentage,
                Address = result.Address,
                Latitude = result.Latitude,
                Longitude = result.Longitude,
                CreatedAtUtc = result.CreatedAtUtc,
                UpdatedAtUtc = result.UpdatedAtUtc
            };
        }

        private static CommercialActivityReportResponse ToResponse(
            this CommercialActivityReportResult result)
        {
            return new CommercialActivityReportResponse
            {
                TimelineExternalId = result.TimelineExternalId,
                ProjectExternalId = result.ProjectExternalId,
                ProjectName = result.ProjectName,
                ProjectStatusId = result.ProjectStatusId,
                ProjectStatusName = result.ProjectStatusName,
                SellerExternalId = result.SellerExternalId,
                SellerName = result.SellerName,
                EventTypeId = result.EventTypeId,
                EventTypeName = result.EventTypeName,
                Title = result.Title,
                Description = result.Description,
                OccurredAtUtc = result.OccurredAtUtc,
                CreatedByExternalId = result.CreatedByExternalId,
                CreatedByName = result.CreatedByName,
                RelatedEntityType = result.RelatedEntityType,
                RelatedEntityId = result.RelatedEntityId,
                MetadataJson = result.MetadataJson
            };
        }

        private static PaginationResponse ToPaginationResponse<T>(this ReportPagedList<T> result)
        {
            return new PaginationResponse
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };
        }
    }
}
