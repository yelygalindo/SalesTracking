namespace UrbanTrack.Api.Controllers.Requests.Customers
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RegisterByExternalId { get; set; }
        public int StatusId { get; set; } = 1;
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}