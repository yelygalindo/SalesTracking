namespace UrbanTrack.Api.Controllers.Responses.Deliveries
{
    public sealed class DeliveryStatusResponse
    {
        public int DeliveryStatusId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
