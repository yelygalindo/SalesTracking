using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Dashboard.Comands;
using SalesTracking.Application.UseCases.Dashboard.Interfaces;
using SalesTracking.Application.UseCases.Dashboard.Results;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Dashboard
{
    public sealed class DashboardRepository : IDashboardRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly ICurrentUser _currentUser;

        public DashboardRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private int CompanyId => _currentUser.CompanyId;
        private bool IsSeller => _currentUser.Roles.Contains("seller", StringComparer.OrdinalIgnoreCase);

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<DashboardResult> GetAsync(GetDashboardCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var parameters = new
            {
                SellerExternalId = IsSeller ? _currentUser.UserExternalId : command.SellerExternalId,
                command.StatusId,
                CompanyId
            };

            using SqlMapper.GridReader results = await connection.QueryMultipleAsync(
                DashboardQueries.GetDashboard,
                parameters);

            DashboardMetricsRow metrics = await results.ReadSingleAsync<DashboardMetricsRow>();
            var projectLocations = await results.ReadAsync<DashboardProjectLocationRow>();
            var recentActivity = await results.ReadAsync<DashboardRecentActivityRow>();
            var upcomingFollowUps = await results.ReadAsync<DashboardUpcomingFollowUpRow>();
            var urgentDeliveries = await results.ReadAsync<DashboardUrgentDeliveryRow>();

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
