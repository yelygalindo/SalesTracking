namespace SalesTracking.Application.UseCases.Customers.Models
{
    public class UpdateCustomer
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? SellerId { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Message { get; set; }
        public int UpdatedByUserId { get; set; }
    }
}
