namespace UrbanTrack.Api.Controllers.Requests.Products
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
}