namespace SalesTracking.Application.UseCases.Products.Results
{
    public sealed class CreateProductResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}