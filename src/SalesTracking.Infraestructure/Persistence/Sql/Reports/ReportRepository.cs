using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Reports.Comands;
using SalesTracking.Application.UseCases.Reports.Interfaces;
using SalesTracking.Application.UseCases.Reports.Models;
using SalesTracking.Application.UseCases.Reports.Results;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Reports.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Reports.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Reports
{
    public sealed class ReportRepository : IReportRepository
    {
        private readonly DatabaseSettings _databaseOptions;

        public ReportRepository(IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<ReportPagedList<DeliveryReportResult>> GetDeliveriesAsync(GetReportCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var rows = (await connection.QueryAsync<DeliveryReportRow>(
                ReportRepositoryQueries.GetDeliveries,
                ToParameters(command))).ToList();

            return ToPagedList(rows.Select(x => x.ToResult()).ToList(), command, rows.FirstOrDefault()?.TotalCount ?? 0);
        }

        public async Task<ReportPagedList<CustomerPendingContactReportResult>> GetCustomersPendingContactAsync(GetReportCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var rows = (await connection.QueryAsync<CustomerPendingContactReportRow>(
                ReportRepositoryQueries.GetCustomersPendingContact,
                ToParameters(command))).ToList();

            return ToPagedList(rows.Select(x => x.ToResult()).ToList(), command, rows.FirstOrDefault()?.TotalCount ?? 0);
        }

        public async Task<ReportPagedList<ProjectReportResult>> GetProjectsAsync(GetReportCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var rows = (await connection.QueryAsync<ProjectReportRow>(
                ReportRepositoryQueries.GetProjects,
                ToParameters(command))).ToList();

            return ToPagedList(rows.Select(x => x.ToResult()).ToList(), command, rows.FirstOrDefault()?.TotalCount ?? 0);
        }

        public async Task<ReportPagedList<CommercialActivityReportResult>> GetCommercialActivityAsync(GetReportCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var rows = (await connection.QueryAsync<CommercialActivityReportRow>(
                ReportRepositoryQueries.GetCommercialActivity,
                ToParameters(command))).ToList();

            return ToPagedList(rows.Select(x => x.ToResult()).ToList(), command, rows.FirstOrDefault()?.TotalCount ?? 0);
        }

        private static object ToParameters(GetReportCommand command)
        {
            return new
            {
                command.From,
                command.To,
                command.SellerExternalId,
                command.StatusId,
                Offset = (command.Page - 1) * command.PageSize,
                command.PageSize
            };
        }

        private static ReportPagedList<T> ToPagedList<T>(
            IReadOnlyCollection<T> items,
            GetReportCommand command,
            int totalItems)
        {
            return new ReportPagedList<T>
            {
                Items = items,
                Page = command.Page,
                PageSize = command.PageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0
                    ? 0
                    : (int)Math.Ceiling(totalItems / (double)command.PageSize)
            };
        }
    }
}
