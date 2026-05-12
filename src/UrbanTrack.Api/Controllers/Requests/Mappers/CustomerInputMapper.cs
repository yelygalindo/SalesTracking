using SalesTracking.Application.UseCases.Customers.Comands;
using UrbanTrack.Api.Controllers.Requests.Customers;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class CustomerInputMapper
    {
        public static CreateCustomerCommand ToApplication(this CreateCustomerRequest request)
        {
            return new CreateCustomerCommand
            {
                Name = request.Name,
                CompanyName = request.CompanyName,
                Phone = request.Phone,
                Email = request.Email,
                RegisterByExternalId = request.RegisterByExternalId,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude
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

        public static UpdateCustomerCommand ToApplication(this UpdateCustomerRequest request, int customerId)
        {
            return new UpdateCustomerCommand
            {
                CustomerId = customerId,
                Name = request.Name,
                CompanyName = request.CompanyName,
                Phone = request.Phone,
                Email = request.Email,
                RegisterByExternalId = request.RegisterByExternalId,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };
        }

        public static ChangeCustomerStatusCommand ToApplication(this ChangeStatusRequest request, int customerId)
        {
            return new ChangeCustomerStatusCommand
            {
                CustomerId = customerId,
                StatusId = request.StatusId
            };
        }
    }
}
