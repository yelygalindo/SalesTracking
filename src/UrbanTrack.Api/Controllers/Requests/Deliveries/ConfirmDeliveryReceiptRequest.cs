namespace UrbanTrack.Api.Controllers.Requests.Deliveries
{
    public sealed class ConfirmDeliveryReceiptRequest
    {
        public DateTime ReceivedAtUtc { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyList<ConfirmDeliveryReceiptItemRequest> Items { get; set; } = new List<ConfirmDeliveryReceiptItemRequest>();
    }

    public sealed class ConfirmDeliveryReceiptItemRequest
    {
        public string DeliveryItemExternalId { get; set; } = string.Empty;
        public decimal ReceivedQuantity { get; set; }
    }
}
