namespace SalesTracking.Application.UseCases.Products.Results
{
    public sealed class UpdateProductResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}