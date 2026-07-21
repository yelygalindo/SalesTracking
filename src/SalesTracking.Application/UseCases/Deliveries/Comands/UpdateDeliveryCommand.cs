namespace SalesTracking.Application.UseCases.Deliveries.Comands
{
    public sealed class UpdateDeliveryCommand
    {
        public string ExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyList<UpdateDeliveryItemCommand> Items { get; set; } = new List<UpdateDeliveryItemCommand>();
    }

    public sealed class UpdateDeliveryItemCommand
    {
        public string ProductExternalId { get; set; } = string.Empty;
        public string UnitExternalId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
    }
}