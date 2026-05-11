namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public class GetCustomersResponse
    {
        public IReadOnlyList<CustomerSummaryResponse> Customers { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}