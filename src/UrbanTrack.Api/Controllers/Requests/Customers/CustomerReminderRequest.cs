using System;

namespace UrbanTrack.Api.Controllers.Requests.Customers
{
    public class CustomerReminderRequest
    {
        public string Text { get; set; }
        public DateTime ReminderAt { get; set; }
        public string AssignedToId { get; set; }
    }
}