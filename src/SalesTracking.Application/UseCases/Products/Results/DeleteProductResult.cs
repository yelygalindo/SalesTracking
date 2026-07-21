namespace SalesTracking.Application.UseCases.Products.Results
{
    public sealed class DeleteProductResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}