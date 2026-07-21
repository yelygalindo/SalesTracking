using SalesTracking.Domain.Enums;

namespace SalesTracking.Application.UseCases.Customers.Models
{
    public class CreateCustomer
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? RegisterByExternalId { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public CustomerStatus Status { get; set; }
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
