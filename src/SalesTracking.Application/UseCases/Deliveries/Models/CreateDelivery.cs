namespace SalesTracking.Application.UseCases.Deliveries.Models
{
    public sealed class CreateDelivery
    {
        public string ExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
        public IReadOnlyList<CreateDeliveryItem> Items { get; set; } = new List<CreateDeliveryItem>();
    }

    public sealed class CreateDeliveryItem
    {
        public string ExternalId { get; set; } = string.Empty;
        public string ProductExternalId { get; set; } = string.Empty;
        public string UnitExternalId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
    }
}