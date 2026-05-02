using System;

namespace UrbanTrack.Api.Controllers.Requests.Customers
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string SellerId { get; set; }
        public string Status { get; set; } = "active";
        public DateTime? FoundedAt { get; set; }
    }
}