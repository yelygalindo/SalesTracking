namespace SalesTracking.Application.UseCases.Customers.Comands
{
    public record GetCustomersCommand
    {
        public string? Status { get; set; }
        public string? SellerExternalId { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}