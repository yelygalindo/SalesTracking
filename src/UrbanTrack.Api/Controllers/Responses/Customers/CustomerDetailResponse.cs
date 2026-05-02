using System;
using System.Collections.Generic;

namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public class CustomerDetailResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string SellerId { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<CustomerNoteResponse> Notes { get; set; } = new List<CustomerNoteResponse>();
    }

    public class CustomerNoteResponse
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerReminderResponse
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime ReminderAt { get; set; }
        public string AssignedToId { get; set; }
        public bool Completed { get; set; }
    }
}