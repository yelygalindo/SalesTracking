namespace SalesTracking.Application.UseCases.Products.Models
{
    public sealed class CreateProduct
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ExternalUnitId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
