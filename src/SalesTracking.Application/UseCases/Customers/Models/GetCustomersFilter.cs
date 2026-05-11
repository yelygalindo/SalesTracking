using SalesTracking.Domain.Enums;

namespace SalesTracking.Application.UseCases.Customers.Models
{
    public class GetCustomersFilter
    {
        public CustomerStatus? Status { get; set; }
        public string? SellerExternalId { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
