using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Companies.Interfaces;
using SalesTracking.Application.UseCases.Companies.Models;
using SalesTracking.Application.UseCases.Companies.Results;
using SalesTracking.Infrastructure.Persistence.Settings;

namespace SalesTracking.Infrastructure.Persistence.Sql.Companies;

public sealed class CompanyRepository : ICompanyRepository
{
    private readonly DatabaseSettings _databaseSettings;
    private readonly IPasswordHasher _passwordHasher;

    public CompanyRepository(
        IOptions<DatabaseSettings> databaseSettings,
        IPasswordHasher passwordHasher)
    {
        _databaseSettings = databaseSettings.Value;
        _passwordHasher = passwordHasher;
    }

    private IDbConnection CreateConnection() =>
        new SqlConnection(_databaseSettings.ConnectionString);

    public async Task<RegisterCompanyResult> RegisterAsync(RegisterCompany company)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbTransaction transaction =
            connection.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            if (await connection.ExecuteScalarAsync<int>(
                    CompanyRepositoryQueries.CompanyNameExists,
                    new { company.CompanyName },
                    transaction) > 0)
            {
                transaction.Rollback();
                return Failure("Ya existe una compañía con ese nombre.");
            }

            if (await connection.ExecuteScalarAsync<int>(
                    CompanyRepositoryQueries.EmailExists,
                    new { company.AdminEmail },
                    transaction) > 0)
            {
                transaction.Rollback();
                return Failure("El correo del administrador ya está registrado.");
            }

            int? roleId = await connection.QuerySingleOrDefaultAsync<int?>(
                CompanyRepositoryQueries.GetAdminRoleId,
                transaction: transaction);
            if (roleId == null)
            {
                transaction.Rollback();
                return Failure("El rol administrador no está configurado.");
            }

            int companyId = await connection.QuerySingleAsync<int>(
                CompanyRepositoryQueries.InsertCompany,
                company,
                transaction);

            int userId = await connection.QuerySingleAsync<int>(
                CompanyRepositoryQueries.InsertAdminUser,
                new
                {
                    company.AdminUserExternalId,
                    company.AdminUsername,
                    company.AdminEmail,
                    company.AdminFullName,
                    PasswordHash = _passwordHasher.Hash(company.Password),
                    CompanyId = companyId
                },
                transaction);

            await connection.ExecuteAsync(
                CompanyRepositoryQueries.InsertUserRole,
                new { UserId = userId, RoleId = roleId.Value },
                transaction);

            transaction.Commit();
            return new RegisterCompanyResult
            {
                Succeeded = true,
                CompanyExternalId = company.CompanyExternalId,
                AdminUserExternalId = company.AdminUserExternalId,
                Message = "Compañía y administrador creados correctamente."
            };
        }
        catch (Exception exception) when (
            SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
        {
            transaction.Rollback();
            return Failure("Ocurrió un error al registrar la compañía.");
        }
    }

    private static RegisterCompanyResult Failure(string message) =>
        new() { Succeeded = false, Message = message };
}
