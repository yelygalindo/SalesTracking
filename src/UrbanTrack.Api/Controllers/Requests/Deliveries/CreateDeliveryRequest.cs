namespace UrbanTrack.Api.Controllers.Requests.Deliveries
{
    public sealed class CreateDeliveryRequest
    {
        public string ProjectExternalId { get; set; } = string.Empty;
        public DateTime CommittedDateUtc { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyList<CreateDeliveryItemRequest> Items { get; set; } = new List<CreateDeliveryItemRequest>();
    }

    public sealed class CreateDeliveryItemRequest
    {
        public string ProductExternalId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
}
