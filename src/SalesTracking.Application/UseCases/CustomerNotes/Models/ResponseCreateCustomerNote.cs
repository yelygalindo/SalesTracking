namespace SalesTracking.Application.UseCases.CustomerNotes.Models
{
    public class ResponseCreateCustomerNote
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Message { get; set; }
        public CreateCustomerNote CreateCustomerNote { get; set; }
    }
}