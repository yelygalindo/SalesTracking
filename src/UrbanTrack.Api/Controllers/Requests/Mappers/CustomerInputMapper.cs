using SalesTracking.Application.UseCases.Customers.Comands;
using UrbanTrack.Api.Controllers.Requests.Customers;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class CustomerInputMapper
    {
        public static CreateCustomerCommand ToApplication(this CreateCustomerRequest request, int createdByUserId)
        {
            return new CreateCustomerCommand
            {
                Name = request.Name,
                CompanyName = request.CompanyName,
                Phone = request.Phone,
                Email = request.Email,
                SellerExternalId = request.SellerExternalId,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CreatedByUserId = createdByUserId
            };
        }

        public static GetCustomersCommand ToApplication(this GetCustomersRequest getCustomersRequest)
        {
            return new GetCustomersCommand
            {
                Status = getCustomersRequest.Status,
                SellerExternalId = getCustomersRequest.ExternalUserId,
                Search = getCustomersRequest.Search,
                Page = getCustomersRequest.Page,
                PageSize = getCustomersRequest.PageSize
            };
        }

        public static UpdateCustomerCommand ToApplication(
            this UpdateCustomerRequest request,
            string externalId,
            int updatedByUserId)
        {
            return new UpdateCustomerCommand
            {
                ExternalId = externalId,
                Name = request.Name,
                CompanyName = request.CompanyName,
                Phone = request.Phone,
                Email = request.Email,
                SellerExternalId = request.SellerExternalId,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                UpdatedByUserId = updatedByUserId
            };
        }

        public static ChangeCustomerStatusCommand ToApplication(
            this ChangeStatusRequest request,
            string externalId,
            int changedByUserId)
        {
            return new ChangeCustomerStatusCommand
            {
                ExternalId = externalId,
                StatusId = request.StatusId,
                ChangedByUserId = changedByUserId
            };
        }
    }
}
