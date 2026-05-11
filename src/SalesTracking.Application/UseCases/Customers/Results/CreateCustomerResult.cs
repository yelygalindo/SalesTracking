namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class CreateCustomerResult
    {
        public bool Succeeded { get; set; }
        public string? Id { get; set; }
        public string Message { get; set; }
    }
}
