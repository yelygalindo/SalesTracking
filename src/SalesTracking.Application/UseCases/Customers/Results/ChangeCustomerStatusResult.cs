namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class ChangeCustomerStatusResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; }
    }
}
