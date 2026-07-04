using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Infrastructure.Persistence.Sql.Projects.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.Projects.Mappers
{
    internal static class ProjectRepositoryMapper
    {
        public static ProjectDetailResult ToResult(this ProjectDetailRow row)
        {
            return new ProjectDetailResult
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Name = row.Name,
                Description = row.Description,
                CustomerExternalId = row.CustomerExternalId,
                CustomerName = row.CustomerName,
                SellerExternalId = row.SellerExternalId,
                SellerName = row.SellerName,
                Status = row.StatusName,
                EstimatedAmount = row.EstimatedAmount,
                StartDateUtc = row.StartDateUtc,
                ExpectedCloseDateUtc = row.ExpectedCloseDateUtc,
                ProgressPercentage = row.ProgressPercentage,
                ActualCloseDateUtc = row.ActualCloseDateUtc,
                Address = row.Address,
                Latitude = row.Latitude,
                Longitude = row.Longitude,
                CreatedAtUtc = row.CreatedAtUtc
            };
        }

        public static ProjectSummaryResult ToResult(this ProjectSummaryRow row)
        {
            return new ProjectSummaryResult
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Name = row.Name,
                Description = row.Description,
                CustomerId = row.CustomerExternalId,
                CustomerName = row.CustomerName,
                SellerId = row.SellerExternalId,
                SellerName = row.SellerName,
                Status = row.StatusName,
                EstimatedAmount = row.EstimatedAmount,
                StartDateUtc = row.StartDateUtc,
                ExpectedCloseDateUtc = row.ExpectedCloseDateUtc,
                ProgressPercentage = row.ProgressPercentage,
                ActualCloseDateUtc = row.ActualCloseDateUtc,
                Address = row.Address,
                Latitude = row.Latitude,
                Longitude = row.Longitude,
                CreatedAtUtc = row.CreatedAtUtc
            };
        }
    }
}
