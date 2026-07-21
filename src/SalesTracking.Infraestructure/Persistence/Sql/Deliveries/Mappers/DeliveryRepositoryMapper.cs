using SalesTracking.Application.UseCases.Deliveries.Results;
using SalesTracking.Infrastructure.Persistence.Sql.Deliveries.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.Deliveries.Mappers
{
    internal static class DeliveryRepositoryMapper
    {
        public static DeliveryStatusResult ToResult(this DeliveryStatusRow row)
        {
            return new DeliveryStatusResult
            {
                DeliveryStatusId = row.DeliveryStatusId,
                Name = row.Name,
                Description = row.Description,
                IsActive = row.IsActive
            };
        }

        public static DeliveryItemResult ToResult(this DeliveryItemRow row)
        {
            return new DeliveryItemResult
            {
                ExternalId = row.ExternalId,
                ProductExternalId = row.ProductExternalId,
                ProductName = row.ProductName,
                UnitExternalId = row.UnitExternalId,
                UnitName = row.UnitName,
                Quantity = row.Quantity,
                DeliveredQuantity = row.DeliveredQuantity
            };
        }

        public static DeliveryResult ToResult(this DeliveryRow row, IReadOnlyList<DeliveryItemResult> items)
        {
            return new DeliveryResult
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                ProjectExternalId = row.ProjectExternalId,
                ProjectName = row.ProjectName,
                SellerExternalId = row.SellerExternalId,
                SellerName = row.SellerName,
                StatusId = row.StatusId,
                StatusName = row.StatusName,
                CommittedDateUtc = row.CommittedDateUtc,
                DeliveredDateUtc = row.DeliveredDateUtc,
                Notes = row.Notes,
                CreatedAtUtc = row.CreatedAtUtc,
                UpdatedAtUtc = row.UpdatedAtUtc,
                Items = items
            };
        }
    }
}
