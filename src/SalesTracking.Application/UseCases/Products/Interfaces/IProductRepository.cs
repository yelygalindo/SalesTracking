using SalesTracking.Application.UseCases.Products.Comands;
using SalesTracking.Application.UseCases.Products.Models;
using SalesTracking.Application.UseCases.Products.Results;

namespace SalesTracking.Application.UseCases.Products.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductPagedList> GetAsync(GetProductsCommand command);
        Task<ProductResult?> GetByExternalIdAsync(string externalId);
        Task<CreateProductResult> CreateAsync(CreateProduct product);
        Task<UpdateProductResult> UpdateAsync(UpdateProductCommand command);
        Task<DeleteProductResult> DeleteAsync(string externalId);
    }
}