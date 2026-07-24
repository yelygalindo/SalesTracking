namespace UrbanTrack.Api.Controllers.Responses.Products
{
    public sealed class ProductResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ExternalUnitId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
