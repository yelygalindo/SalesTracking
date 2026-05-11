namespace SalesTracking.Application.UseCases.Customers.Comands
{
    public class CreateCustomerCommand
    {
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? RegisterByExternalId { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
