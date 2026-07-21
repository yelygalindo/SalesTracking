namespace SalesTracking.Application.UseCases.Deliveries.Results
{
    public sealed class DeliveryStatusResult
    {
        public int DeliveryStatusId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}