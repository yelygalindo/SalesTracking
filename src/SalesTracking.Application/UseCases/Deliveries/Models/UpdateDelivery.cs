namespace SalesTracking.Application.UseCases.Deliveries.Models
{
    public sealed class UpdateDelivery
    {
        public string ExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyList<CreateDeliveryItem> Items { get; set; } = new List<CreateDeliveryItem>();
    }
}