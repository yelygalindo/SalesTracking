namespace SalesTracking.Application.UseCases.Deliveries.Results
{
    public sealed class CreateDeliveryResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}