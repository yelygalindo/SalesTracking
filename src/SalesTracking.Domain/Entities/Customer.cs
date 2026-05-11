using SalesTracking.Domain.Enums;

namespace SalesTracking.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public CustomerStatus Status { get; set; }        
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public Seller? Seller { get; set; }
        public List<CustomerNote> Notes { get; set; } = new();
    }
}
