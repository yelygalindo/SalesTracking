using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Authorization;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Infrastructure.Persistence.Settings;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Authorization;

public sealed class UserAuthorizationRepository : IUserAuthorizationRepository
{
    private readonly string _connectionString;

    public UserAuthorizationRepository(IOptions<DatabaseSettings> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<UserAuthorizationInfo> GetByUserIdAsync(int userId)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        using SqlMapper.GridReader result = await connection.QueryMultipleAsync(UserAuthorizationQueries.Get, new { UserId = userId });
        string[] roles = (await result.ReadAsync<string>()).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        string[] permissions = (await result.ReadAsync<string>()).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        return new UserAuthorizationInfo(roles, permissions);
    }
}
