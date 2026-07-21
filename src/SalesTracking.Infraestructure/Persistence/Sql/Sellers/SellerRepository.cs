using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Sellers.Interfaces;
using SalesTracking.Application.UseCases.Sellers.Results;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Sellers.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Sellers;

public sealed class SellerRepository : ISellerRepository
{
    private readonly string _connectionString;

    public SellerRepository(IOptions<DatabaseSettings> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<IReadOnlyList<SellerResult>> GetActiveAsync(int companyId)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        IEnumerable<SellerRow> rows = await connection.QueryAsync<SellerRow>(
            SellerQueries.GetActive,
            new { CompanyId = companyId });

        return rows.Select(row => new SellerResult
        {
            ExternalId = row.ExternalId,
            DisplayName = row.DisplayName,
            Email = row.Email
        }).ToList();
    }
}
