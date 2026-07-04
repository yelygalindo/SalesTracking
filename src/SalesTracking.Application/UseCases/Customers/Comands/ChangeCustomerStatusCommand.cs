namespace SalesTracking.Application.UseCases.Customers.Comands
{
    public class ChangeCustomerStatusCommand
    {
        public string ExternalId { get; set; } = string.Empty;
        public int StatusId { get; set; }
    }
}
