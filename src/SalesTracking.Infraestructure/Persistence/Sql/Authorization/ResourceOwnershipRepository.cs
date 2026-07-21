using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Infrastructure.Persistence.Settings;

namespace SalesTracking.Infrastructure.Persistence.Sql.Authorization;

public sealed class ResourceOwnershipRepository : IResourceOwnershipRepository
{
    private readonly string _connectionString;
    public ResourceOwnershipRepository(IOptions<DatabaseSettings> options) => _connectionString = options.Value.ConnectionString;

    public async Task<bool> IsAssignedToSellerAsync(string resource, string externalId, int companyId, int sellerUserId)
    {
        string? query = resource switch
        {
            "customer" => @"SELECT COUNT(1) FROM Customers WHERE ExternalId=@ExternalId AND CompanyId=@CompanyId AND SellerId=@SellerUserId AND IsDeleted=0;",
            "project" => @"SELECT COUNT(1) FROM Projects WHERE ExternalId=@ExternalId AND CompanyId=@CompanyId AND SellerId=@SellerUserId AND IsDeleted=0;",
            "delivery" => @"SELECT COUNT(1) FROM Deliveries WHERE ExternalId=@ExternalId AND CompanyId=@CompanyId AND SellerId=@SellerUserId AND IsDeleted=0;",
            _ => null
        };
        if (query == null) return true;
        await using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(query, new { ExternalId = externalId, CompanyId = companyId, SellerUserId = sellerUserId }) > 0;
    }
}
