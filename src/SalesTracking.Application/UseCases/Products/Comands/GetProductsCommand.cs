namespace SalesTracking.Application.UseCases.Products.Comands
{
    public sealed record GetProductsCommand(
        string? Search,
        int Page,
        int PageSize);
}