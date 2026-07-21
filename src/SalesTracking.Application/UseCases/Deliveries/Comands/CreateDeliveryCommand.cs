namespace SalesTracking.Application.UseCases.Deliveries.Comands
{
    public sealed class CreateDeliveryCommand
    {
        public string ProjectExternalId { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
        public IReadOnlyList<CreateDeliveryItemCommand> Items { get; set; } = new List<CreateDeliveryItemCommand>();
    }

    public sealed class CreateDeliveryItemCommand
    {
        public string ProductExternalId { get; set; } = string.Empty;
        public string UnitExternalId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
    }
}