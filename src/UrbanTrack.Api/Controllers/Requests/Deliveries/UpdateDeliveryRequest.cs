namespace UrbanTrack.Api.Controllers.Requests.Deliveries
{
    public sealed class UpdateDeliveryRequest
    {
        public string ProjectExternalId { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyList<UpdateDeliveryItemRequest> Items { get; set; } = new List<UpdateDeliveryItemRequest>();
    }

    public sealed class UpdateDeliveryItemRequest
    {
        public string ProductExternalId { get; set; } = string.Empty;
        public string UnitExternalId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
    }
}
