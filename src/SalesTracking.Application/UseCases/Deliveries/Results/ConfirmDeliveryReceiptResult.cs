namespace SalesTracking.Application.UseCases.Deliveries.Results
{
    public sealed class ConfirmDeliveryReceiptResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
