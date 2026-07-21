namespace UrbanTrack.Api.Controllers.Requests.Products
{
    public sealed class GetProductsRequest
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}