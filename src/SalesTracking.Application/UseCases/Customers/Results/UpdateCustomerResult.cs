namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class UpdateCustomerResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; }
    }
}