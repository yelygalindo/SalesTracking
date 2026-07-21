using SalesTracking.Application.UseCases.Products.Comands;
using SalesTracking.Application.UseCases.Products.Models;
using SalesTracking.Application.UseCases.Products.Results;

namespace SalesTracking.Application.UseCases.Products.Interfaces
{
    public interface IProductService
    {
        Task<ProductPagedList> GetAsync(GetProductsCommand command);
        Task<ProductResult?> GetByExternalIdAsync(GetProductByExternalIdCommand command);
        Task<CreateProductResult> CreateAsync(CreateProductCommand command);
        Task<UpdateProductResult> UpdateAsync(UpdateProductCommand command);
        Task<DeleteProductResult> DeleteAsync(DeleteProductCommand command);
    }
}