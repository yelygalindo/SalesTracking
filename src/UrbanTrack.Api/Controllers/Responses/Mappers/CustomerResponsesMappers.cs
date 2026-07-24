using SalesTracking.Application.UseCases.Customers.Results;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.CustomerNotes;
using UrbanTrack.Api.Controllers.Responses.Customers;

namespace UrbanTrack.Api.Controllers.Responses.Mappers
{
    internal static class CustomerResponsesMappers
    {
        public static CustomerDetailResponse ToResponse(this CustomerDetailResult customerDetailResult)
        {
            if (customerDetailResult == null) return null;
            return new CustomerDetailResponse()
            {
                Id = customerDetailResult.Id,
                ExternalId = customerDetailResult.ExternalId,
                Name = customerDetailResult.Name,
                CompanyName = customerDetailResult.CompanyName,
                Phone = customerDetailResult.Phone,
                Email = customerDetailResult.Email,
                StatusId = (int)customerDetailResult.Status,
                Status = customerDetailResult.Status.ToApiValue(),
                Seller = new UserResponse(customerDetailResult.SellerResult?.Id, customerDetailResult.SellerResult?.ExternalId, customerDetailResult.SellerResult?.Name),               
                Address = customerDetailResult.Address,
                Latitude = customerDetailResult.Latitude,
                Longitude = customerDetailResult.Longitude,
                CreatedAt = customerDetailResult.CreatedAtUtc,
                Notes = customerDetailResult.Notes.Select(x=>x.ToResponse()),
                Reminders = customerDetailResult.Reminders.Select(x => x.ToResponse())
            };
        }
        
        public static CustomerStatusResponse ToResponse(this CustomerStatusResult customerStatusResult)
        {
            if (customerStatusResult == null) return null;
            return new CustomerStatusResponse()
            {
                Value = customerStatusResult.Value,
                Label = customerStatusResult.Label
            };
        }
        public static CustomerNoteResponse ToResponse(this CustomerNoteResult customerNoteResult)
        {
            if (customerNoteResult == null) return null;
            return new CustomerNoteResponse()
            {                
                Id = customerNoteResult.Id,
                ExternalId = customerNoteResult.ExternalId,
                Text = customerNoteResult.Text,
                Author = new UserResponse(customerNoteResult.Author.Id, customerNoteResult.Author.ExternalId, customerNoteResult.Author.Name),
                CreatedAt = customerNoteResult.CreatedAtUtc,
            };
        }

        public static GetCustomersResponse ToResponse(this GetCustomersResult getCustomersResult)
        {
            if (getCustomersResult == null) return null;
            return new GetCustomersResponse()
            {
                Customers = getCustomersResult.Items.Select(x => x.ToResponse()).ToList(),
                Page = getCustomersResult.Page,
                PageSize = getCustomersResult.PageSize,
                TotalItems = getCustomersResult.TotalItems,
                TotalPages = getCustomersResult.TotalPages,
            };
        }

        public static CustomerSummaryResponse ToResponse(this CustomerSummaryResult customerSummaryResult)
        {
            if (customerSummaryResult == null) return null;
            return new CustomerSummaryResponse()
            {
                Id = customerSummaryResult.Id,
                ExternalId = customerSummaryResult.ExternalId,
                Name = customerSummaryResult.Name,
                CompanyName = customerSummaryResult.CompanyName,
                Phone = customerSummaryResult.Phone,
                Email = customerSummaryResult.Email,
                Status = customerSummaryResult.Status.ToApiValue(),
                CreatedAt = customerSummaryResult.CreatedAtUtc,
                Seller = new UserResponse((int)(customerSummaryResult.SellerResult?.Id), customerSummaryResult.SellerResult?.ExternalId, customerSummaryResult.SellerResult?.Name)                
            };
        }

        public static IdMessageResponse ToResponse(this CreateCustomerResult result)
        {
            return new IdMessageResponse
            {
                Id = result.Id,
                Message = result.Message
            };
        }
    }
}
