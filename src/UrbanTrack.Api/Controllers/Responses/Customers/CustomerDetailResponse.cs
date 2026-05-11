namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public class CustomerDetailResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }        
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public SellerResponse Seller { get; set; }
        public IEnumerable<CustomerNoteResponse> Notes { get; set; } = new List<CustomerNoteResponse>();
    }
}