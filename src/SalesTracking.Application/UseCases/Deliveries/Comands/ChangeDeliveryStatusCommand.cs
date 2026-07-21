namespace SalesTracking.Application.UseCases.Deliveries.Comands
{
    public sealed class ChangeDeliveryStatusCommand
    {
        public string ExternalId { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public int ChangedByUserId { get; set; }
    }
}