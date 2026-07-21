using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Dashboard.Comands;
using SalesTracking.Application.UseCases.Dashboard.Interfaces;
using SalesTracking.Application.UseCases.Dashboard.Results;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Dashboard
{
    public sealed class DashboardRepository : IDashboardRepository
    {
        private readonly DatabaseSettings _databaseOptions;

        public DashboardRepository(IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<DashboardResult> GetAsync(GetDashboardCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var parameters = new
            {
                command.SellerExternalId,
                command.StatusId
            };

            DashboardMetricsRow metrics = await connection.QuerySingleAsync<DashboardMetricsRow>(
                DashboardQueries.GetMetrics,
                parameters);

            var projectLocations = await connection.QueryAsync<DashboardProjectLocationRow>(
                DashboardQueries.GetProjectLocations,
                parameters);

            var recentActivity = await connection.QueryAsync<DashboardRecentActivityRow>(
                DashboardQueries.GetRecentActivity,
                parameters);

            var upcomingFollowUps = await connection.QueryAsync<DashboardUpcomingFollowUpRow>(
                DashboardQueries.GetUpcomingFollowUps,
                parameters);

            var urgentDeliveries = await connection.QueryAsync<DashboardUrgentDeliveryRow>(
                DashboardQueries.GetUrgentDeliveries,
                parameters);

            return new DashboardResult
            {
                Metrics = metrics.ToResult(),
                ProjectLocations = projectLocations.Select(x => x.ToResult()).ToList(),
                RecentActivity = recentActivity.Select(x => x.ToResult()).ToList(),
                UpcomingFollowUps = upcomingFollowUps.Select(x => x.ToResult()).ToList(),
                UrgentDeliveries = urgentDeliveries.Select(x => x.ToResult()).ToList()
            };
        }
    }
}
