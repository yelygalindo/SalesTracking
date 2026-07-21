using SalesTracking.Application.UseCases.Products.Results;

namespace SalesTracking.Application.UseCases.Products.Models
{
    public sealed class ProductPagedList
    {
        public IReadOnlyList<ProductResult> Items { get; set; } = new List<ProductResult>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}