using SalesTracking.Application.UseCases.Deliveries.Comands;
using SalesTracking.Application.UseCases.Deliveries.Models;
using SalesTracking.Application.UseCases.Deliveries.Results;
using UrbanTrack.Api.Controllers.Requests.Deliveries;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Deliveries;
using UrbanTrack.Api.Controllers.Responses.Pagination;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class DeliveryApiMapper
    {
        public static GetDeliveriesCommand ToApplication(this GetDeliveriesRequest request)
        {
            return new GetDeliveriesCommand(request.Page, request.PageSize);
        }

        public static CreateDeliveryCommand ToApplication(this CreateDeliveryRequest request, int createdByUserId)
        {
            return new CreateDeliveryCommand
            {
                ProjectExternalId = request.ProjectExternalId,
                SellerExternalId = request.SellerExternalId,
                CommittedDateUtc = request.CommittedDateUtc,
                DeliveredDateUtc = request.DeliveredDateUtc,
                Notes = request.Notes,
                CreatedByUserId = createdByUserId,
                Items = request.Items.Select(x => new CreateDeliveryItemCommand
                {
                    ProductExternalId = x.ProductExternalId,
                    UnitExternalId = x.UnitExternalId,
                    Quantity = x.Quantity,
                    DeliveredQuantity = x.DeliveredQuantity
                }).ToList()
            };
        }

        public static UpdateDeliveryCommand ToApplication(this UpdateDeliveryRequest request, string externalId)
        {
            return new UpdateDeliveryCommand
            {
                ExternalId = externalId,
                ProjectExternalId = request.ProjectExternalId,
                SellerExternalId = request.SellerExternalId,
                CommittedDateUtc = request.CommittedDateUtc,
                DeliveredDateUtc = request.DeliveredDateUtc,
                Notes = request.Notes,
                Items = request.Items.Select(x => new UpdateDeliveryItemCommand
                {
                    ProductExternalId = x.ProductExternalId,
                    UnitExternalId = x.UnitExternalId,
                    Quantity = x.Quantity,
                    DeliveredQuantity = x.DeliveredQuantity
                }).ToList()
            };
        }

        public static ChangeDeliveryStatusCommand ToApplication(
            this ChangeDeliveryStatusRequest request,
            string externalId,
            int changedByUserId)
        {
            return new ChangeDeliveryStatusCommand
            {
                ExternalId = externalId,
                StatusId = request.StatusId,
                DeliveredDateUtc = request.DeliveredDateUtc,
                ChangedByUserId = changedByUserId
            };
        }

        public static ConfirmDeliveryReceiptCommand ToApplication(
            this ConfirmDeliveryReceiptRequest request,
            string deliveryExternalId,
            int createdByUserId)
        {
            return new ConfirmDeliveryReceiptCommand
            {
                DeliveryExternalId = deliveryExternalId,
                ReceivedAtUtc = request.ReceivedAtUtc,
                Notes = request.Notes,
                CreatedByUserId = createdByUserId,
                Items = request.Items.Select(x => new ConfirmDeliveryReceiptItemCommand
                {
                    DeliveryItemExternalId = x.DeliveryItemExternalId,
                    ReceivedQuantity = x.ReceivedQuantity
                }).ToList()
            };
        }

        public static DeliveryStatusResponse ToResponse(this DeliveryStatusResult result)
        {
            return new DeliveryStatusResponse
            {
                DeliveryStatusId = result.DeliveryStatusId,
                Name = result.Name,
                Description = result.Description,
                IsActive = result.IsActive
            };
        }

        public static DeliveryItemResponse ToResponse(this DeliveryItemResult result)
        {
            return new DeliveryItemResponse
            {
                ExternalId = result.ExternalId,
                ProductExternalId = result.ProductExternalId,
                ProductName = result.ProductName,
                UnitExternalId = result.UnitExternalId,
                UnitName = result.UnitName,
                Quantity = result.Quantity,
                DeliveredQuantity = result.DeliveredQuantity
            };
        }

        public static DeliveryResponse ToResponse(this DeliveryResult result)
        {
            return new DeliveryResponse
            {
                Id = result.Id,
                ExternalId = result.ExternalId,
                ProjectExternalId = result.ProjectExternalId,
                ProjectName = result.ProjectName,
                SellerExternalId = result.SellerExternalId,
                SellerName = result.SellerName,
                StatusId = result.StatusId,
                StatusName = result.StatusName,
                CommittedDateUtc = result.CommittedDateUtc,
                DeliveredDateUtc = result.DeliveredDateUtc,
                Notes = result.Notes,
                CreatedAtUtc = result.CreatedAtUtc,
                UpdatedAtUtc = result.UpdatedAtUtc,
                Items = result.Items.Select(x => x.ToResponse()).ToList()
            };
        }

        public static PagedResponse<DeliveryResponse> ToResponse(this DeliveryPagedList result)
        {
            return new PagedResponse<DeliveryResponse>
            {
                Items = result.Items.Select(x => x.ToResponse()).ToList(),
                Pagination = new PaginationResponse
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                }
            };
        }

        public static IdMessageResponse ToResponse(this CreateDeliveryResult result)
        {
            return new IdMessageResponse
            {
                Id = result.Id,
                Message = result.Message
            };
        }
    }
}