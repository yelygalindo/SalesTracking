using System;

namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public class CustomerSummaryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string SellerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}