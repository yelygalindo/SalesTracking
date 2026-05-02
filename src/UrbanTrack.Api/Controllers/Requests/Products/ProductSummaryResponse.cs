
namespace UrbanTrack.Api.Controllers.Responses.Products
{
    public class ProductSummaryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string CustomerId { get; internal set; }
        public string SellerId { get; internal set; }
        public string Status { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
}