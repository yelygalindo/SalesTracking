using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.Products.Comands;
using SalesTracking.Application.UseCases.Products.Interfaces;
using SalesTracking.Application.UseCases.Products.Models;
using SalesTracking.Application.UseCases.Products.Results;

namespace SalesTracking.Application.UseCases.Products.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductPagedList> GetAsync(GetProductsCommand command)
        {
            int page = command.Page <= 0 ? 1 : command.Page;
            int pageSize = command.PageSize <= 0 ? 20 : command.PageSize;

            if (pageSize > 100)
                pageSize = 100;

            return await _productRepository.GetAsync(
                new GetProductsCommand(
                    string.IsNullOrWhiteSpace(command.Search) ? null : command.Search.Trim(),
                    page,
                    pageSize));
        }

        public async Task<ProductResult?> GetByExternalIdAsync(GetProductByExternalIdCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
                return null;

            return await _productRepository.GetByExternalIdAsync(command.ExternalId.Trim());
        }

        public async Task<CreateProductResult> CreateAsync(CreateProductCommand command)
        {
            CreateProductResult? validation = Validate(command.Code, command.Name, command.ExternalUnitId, command.Price);
            if (validation != null)
                return validation;

            CreateProduct product = new CreateProduct
            {
                ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.Product),
                Code = command.Code.Trim(),
                Name = command.Name.Trim(),
                Description = command.Description?.Trim(),
                ExternalUnitId = command.ExternalUnitId.Trim(),
                Price = command.Price,
                IsActive = command.IsActive
            };

            return await _productRepository.CreateAsync(product);
        }

        public async Task<UpdateProductResult> UpdateAsync(UpdateProductCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new UpdateProductResult
                {
                    Succeeded = false,
                    Message = "El producto es requerido."
                };
            }

            CreateProductResult? validation = Validate(command.Code, command.Name, command.ExternalUnitId, command.Price);
            if (validation != null)
            {
                return new UpdateProductResult
                {
                    Succeeded = false,
                    Message = validation.Message
                };
            }

            UpdateProductCommand normalizedCommand = new UpdateProductCommand
            {
                ExternalId = command.ExternalId.Trim(),
                Code = command.Code.Trim(),
                Name = command.Name.Trim(),
                Description = command.Description?.Trim(),
                ExternalUnitId = command.ExternalUnitId.Trim(),
                Price = command.Price,
                IsActive = command.IsActive
            };

            return await _productRepository.UpdateAsync(normalizedCommand);
        }

        public async Task<DeleteProductResult> DeleteAsync(DeleteProductCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new DeleteProductResult
                {
                    Succeeded = false,
                    Message = "El producto es requerido."
                };
            }

            return await _productRepository.DeleteAsync(command.ExternalId.Trim());
        }

        private static CreateProductResult? Validate(string code, string name, string externalUnitId, decimal price)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new CreateProductResult
                {
                    Succeeded = false,
                    Message = "El código del producto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return new CreateProductResult
                {
                    Succeeded = false,
                    Message = "El nombre del producto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(externalUnitId))
            {
                return new CreateProductResult
                {
                    Succeeded = false,
                    Message = "La unidad del producto es requerida."
                };
            }

            if (price < 0)
            {
                return new CreateProductResult
                {
                    Succeeded = false,
                    Message = "El precio del producto no es válido."
                };
            }

            return null;
        }
    }
}
