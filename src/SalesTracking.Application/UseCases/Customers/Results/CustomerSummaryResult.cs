using SalesTracking.Domain.Enums;

namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class CustomerSummaryResult
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public CustomerStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public SellerResult SellerResult { get; set; }
    }
}
