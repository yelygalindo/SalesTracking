namespace SalesTracking.Application.UseCases.Customers.Comands
{
    public class ChangeCustomerStatusCommand
    {
        public int CustomerId { get; set; }
        public int StatusId { get; set; }
    }
}