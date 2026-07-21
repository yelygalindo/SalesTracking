namespace UrbanTrack.Api.Controllers.Requests.Deliveries
{
    public sealed class ChangeDeliveryStatusRequest
    {
        public int StatusId { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
    }
}
