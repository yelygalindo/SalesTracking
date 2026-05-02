using System;

namespace UrbanTrack.Api.Controllers.Requests.Deliveries
{
    public class CreateDeliveryRequest
    {
        public string ProjectId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string SellerId { get; set; }
        public string Address { get; set; }
    }
}