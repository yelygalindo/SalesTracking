using System;

namespace UrbanTrack.Api.Controllers.Responses.Deliveries
{
    public class DeliverySummaryResponse
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string Status { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string SellerId { get; set; }
    }
}