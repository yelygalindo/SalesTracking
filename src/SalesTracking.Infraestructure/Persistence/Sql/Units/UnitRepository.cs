using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Units.Comands;
using SalesTracking.Application.UseCases.Units.Interfaces;
using SalesTracking.Application.UseCases.Units.Models;
using SalesTracking.Application.UseCases.Units.Results;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Units.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Units.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Units
{
    public sealed class UnitRepository : IUnitRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly ICurrentUser _currentUser;

        public UnitRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private int CompanyId => _currentUser.CompanyId;

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<UnitPagedList> GetAsync(GetUnitsCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var rows = (await connection.QueryAsync<UnitRow>(
                UnitRepositoryQueries.Get,
                new
                {
                    command.Search,
                    Offset = (command.Page - 1) * command.PageSize,
                    command.PageSize,
                    CompanyId
                })).ToList();

            int totalItems = rows.FirstOrDefault()?.TotalCount ?? 0;

            return new UnitPagedList
            {
                Items = rows.Select(x => x.ToResult()).ToList(),
                Page = command.Page,
                PageSize = command.PageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0
                    ? 0
                    : (int)Math.Ceiling(totalItems / (double)command.PageSize)
            };
        }

        public async Task<UnitResult?> GetByExternalIdAsync(string externalId)
        {
            using IDbConnection connection = CreateConnection();

            UnitRow? row = await connection.QuerySingleOrDefaultAsync<UnitRow>(
                UnitRepositoryQueries.GetByExternalId,
                new { ExternalId = externalId, CompanyId });

            return row?.ToResult();
        }

        public async Task<CreateUnitResult> CreateAsync(CreateUnit unit)
        {
            using IDbConnection connection = CreateConnection();

            try
            {
                string? externalId = await connection.QuerySingleOrDefaultAsync<string>(
                    UnitRepositoryQueries.Create,
                    TenantParameters(unit));

                if (string.IsNullOrWhiteSpace(externalId))
                {
                    return new CreateUnitResult
                    {
                        Succeeded = false,
                        Message = "No se pudo crear la unidad."
                    };
                }

                return new CreateUnitResult
                {
                    Succeeded = true,
                    Id = externalId,
                    Message = "Unidad creada correctamente."
                };
            }
            catch
            {
                return new CreateUnitResult
                {
                    Succeeded = false,
                    Message = "Ocurrió un error al crear la unidad."
                };
            }
        }

        public async Task<UpdateUnitResult> UpdateAsync(UpdateUnitCommand command)
        {
            using IDbConnection connection = CreateConnection();

            try
            {
                int affectedRows = await connection.ExecuteAsync(
                    UnitRepositoryQueries.Update,
                    TenantParameters(command));

                if (affectedRows == 0)
                {
                    return new UpdateUnitResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Unidad no encontrada."
                    };
                }

                return new UpdateUnitResult
                {
                    Succeeded = true,
                    Message = "Unidad actualizada correctamente."
                };
            }
            catch
            {
                return new UpdateUnitResult
                {
                    Succeeded = false,
                    Message = "Ocurrió un error al actualizar la unidad."
                };
            }
        }

        public async Task<DeleteUnitResult> DeleteAsync(string externalId)
        {
            using IDbConnection connection = CreateConnection();

            try
            {
                int affectedRows = await connection.ExecuteAsync(
                    UnitRepositoryQueries.Delete,
                    new { ExternalId = externalId, CompanyId });

                if (affectedRows == 0)
                {
                    return new DeleteUnitResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Unidad no encontrada."
                    };
                }

                return new DeleteUnitResult
                {
                    Succeeded = true,
                    Message = "Unidad eliminada correctamente."
                };
            }
            catch
            {
                return new DeleteUnitResult
                {
                    Succeeded = false,
                    Message = "Ocurrió un error al eliminar la unidad."
                };
            }
        }

        private DynamicParameters TenantParameters(object value)
        {
            var parameters = new DynamicParameters(value);
            parameters.Add(nameof(CompanyId), CompanyId);
            return parameters;
        }
    }
}
