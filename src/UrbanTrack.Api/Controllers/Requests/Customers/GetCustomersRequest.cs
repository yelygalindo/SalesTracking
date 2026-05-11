namespace UrbanTrack.Api.Controllers.Requests.Customers
{
    public class GetCustomersRequest
    {
        public string? Status { get; set; }
        public string? ExternalUserId { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}