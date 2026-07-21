using SalesTracking.Application.UseCases.Units.Results;
using SalesTracking.Infrastructure.Persistence.Sql.Units.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.Units.Mappers
{
    internal static class UnitRepositoryMapper
    {
        public static UnitResult ToResult(this UnitRow row)
        {
            return new UnitResult
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Name = row.Name,
                Symbol = row.Symbol,
                Description = row.Description,
                AllowsDecimals = row.AllowsDecimals,
                IsActive = row.IsActive,
                CreatedAtUtc = row.CreatedAtUtc,
                UpdatedAtUtc = row.UpdatedAtUtc
            };
        }
    }
}