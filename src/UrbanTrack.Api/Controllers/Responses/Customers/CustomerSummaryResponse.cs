namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public class CustomerSummaryResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }        
        public DateTime CreatedAt { get; set; }
        public SellerResponse Seller { get; set; }
    }
}