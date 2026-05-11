namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class GetCustomersResult
    {
        public IReadOnlyList<CustomerSummaryResult> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}