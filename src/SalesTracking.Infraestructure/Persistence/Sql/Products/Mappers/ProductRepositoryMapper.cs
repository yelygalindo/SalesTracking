using SalesTracking.Application.UseCases.Products.Results;
using SalesTracking.Infrastructure.Persistence.Sql.Products.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.Products.Mappers
{
    internal static class ProductRepositoryMapper
    {
        public static ProductResult ToResult(this ProductRow row)
        {
            return new ProductResult
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Code = row.Code,
                Name = row.Name,
                Description = row.Description,
                ExternalUnitId = row.ExternalUnitId,
                Price = row.Price,
                IsActive = row.IsActive,
                CreatedAtUtc = row.CreatedAtUtc,
                UpdatedAtUtc = row.UpdatedAtUtc
            };
        }
    }
}
