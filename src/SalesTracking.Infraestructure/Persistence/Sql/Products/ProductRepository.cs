using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Products.Comands;
using SalesTracking.Application.UseCases.Products.Interfaces;
using SalesTracking.Application.UseCases.Products.Models;
using SalesTracking.Application.UseCases.Products.Results;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Products.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Products.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Products
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly ICurrentUser _currentUser;

        public ProductRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private int CompanyId => _currentUser.CompanyId;

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<ProductPagedList> GetAsync(GetProductsCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var rows = (await connection.QueryAsync<ProductRow>(
                ProductRepositoryQueries.Get,
                new
                {
                    command.Search,
                    Offset = (command.Page - 1) * command.PageSize,
                    command.PageSize,
                    CompanyId
                })).ToList();

            int totalItems = rows.FirstOrDefault()?.TotalCount ?? 0;

            return new ProductPagedList
            {
                Items = rows.Select(x => x.ToResult()).ToList(),
                Page = command.Page,
                PageSize = command.PageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0
                    ? 0
                    : (int)Math.Ceiling(totalItems / (double)command.PageSize)
            };
        }

        public async Task<ProductResult?> GetByExternalIdAsync(string externalId)
        {
            using IDbConnection connection = CreateConnection();

            ProductRow? row = await connection.QuerySingleOrDefaultAsync<ProductRow>(
                ProductRepositoryQueries.GetByExternalId,
                new { ExternalId = externalId, CompanyId });

            return row?.ToResult();
        }

        public async Task<CreateProductResult> CreateAsync(CreateProduct product)
        {
            using IDbConnection connection = CreateConnection();

            try
            {
                int? unitId = await connection.QuerySingleOrDefaultAsync<int?>(
                    ProductRepositoryQueries.GetUnitIdByExternalId,
                    new { product.ExternalUnitId, CompanyId });

                if (unitId == null)
                {
                    return new CreateProductResult
                    {
                        Succeeded = false,
                        Message = "La unidad no existe, está inactiva o no pertenece a la compañía."
                    };
                }

                DynamicParameters parameters = TenantParameters(product);
                parameters.Add("UnitId", unitId.Value);

                string? externalId = await connection.QuerySingleOrDefaultAsync<string>(
                    ProductRepositoryQueries.Create,
                    parameters);

                if (string.IsNullOrWhiteSpace(externalId))
                {
                    return new CreateProductResult
                    {
                        Succeeded = false,
                        Message = "No se pudo crear el producto."
                    };
                }

                return new CreateProductResult
                {
                    Succeeded = true,
                    Id = externalId,
                    Message = "Producto creado correctamente."
                };
            }
            catch
            {
                return new CreateProductResult
                {
                    Succeeded = false,
                    Message = "Ocurrió un error al crear el producto."
                };
            }
        }

        public async Task<UpdateProductResult> UpdateAsync(UpdateProductCommand command)
        {
            using IDbConnection connection = CreateConnection();

            try
            {
                int? unitId = await connection.QuerySingleOrDefaultAsync<int?>(
                    ProductRepositoryQueries.GetUnitIdByExternalId,
                    new { command.ExternalUnitId, CompanyId });

                if (unitId == null)
                {
                    return new UpdateProductResult
                    {
                        Succeeded = false,
                        Message = "La unidad no existe, está inactiva o no pertenece a la compañía."
                    };
                }

                DynamicParameters parameters = TenantParameters(command);
                parameters.Add("UnitId", unitId.Value);

                int affectedRows = await connection.ExecuteAsync(
                    ProductRepositoryQueries.Update,
                    parameters);

                if (affectedRows == 0)
                {
                    return new UpdateProductResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Producto no encontrado."
                    };
                }

                return new UpdateProductResult
                {
                    Succeeded = true,
                    Message = "Producto actualizado correctamente."
                };
            }
            catch
            {
                return new UpdateProductResult
                {
                    Succeeded = false,
                    Message = "Ocurrió un error al actualizar el producto."
                };
            }
        }

        public async Task<DeleteProductResult> DeleteAsync(string externalId)
        {
            using IDbConnection connection = CreateConnection();

            try
            {
                int affectedRows = await connection.ExecuteAsync(
                    ProductRepositoryQueries.Delete,
                    new { ExternalId = externalId, CompanyId });

                if (affectedRows == 0)
                {
                    return new DeleteProductResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Producto no encontrado."
                    };
                }

                return new DeleteProductResult
                {
                    Succeeded = true,
                    Message = "Producto eliminado correctamente."
                };
            }
            catch
            {
                return new DeleteProductResult
                {
                    Succeeded = false,
                    Message = "Ocurrió un error al eliminar el producto."
                };
            }
        }

        private DynamicParameters TenantParameters(object value)
        {
            var parameters = new DynamicParameters(value);
            parameters.Add(nameof(CompanyId), CompanyId);
            return parameters;
        }
    }
}
