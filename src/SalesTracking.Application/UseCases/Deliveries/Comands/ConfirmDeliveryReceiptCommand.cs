namespace SalesTracking.Application.UseCases.Deliveries.Comands
{
    public sealed class ConfirmDeliveryReceiptCommand
    {
        public string DeliveryExternalId { get; set; } = string.Empty;
        public DateTime ReceivedAtUtc { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyList<ConfirmDeliveryReceiptItemCommand> Items { get; set; } = new List<ConfirmDeliveryReceiptItemCommand>();
    }

    public sealed class ConfirmDeliveryReceiptItemCommand
    {
        public string DeliveryItemExternalId { get; set; } = string.Empty;
        public decimal ReceivedQuantity { get; set; }
    }
}
