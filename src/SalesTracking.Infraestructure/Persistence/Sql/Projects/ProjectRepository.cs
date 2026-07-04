using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Interfaces;
using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Domain.Entities;
using SalesTracking.Domain.Enums;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Projects.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Projects.Rows;
using System.Data;
namespace SalesTracking.Infrastructure.Persistence.Sql.Projects
{
    public sealed class ProjectRepository : IProjectRepository
    {
        private readonly DatabaseSettings _databaseOptions;

        public ProjectRepository(IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<string?> CreateAsync(Project project)
        {
            using var connection = CreateConnection();

            var createdId = await connection.QuerySingleOrDefaultAsync<string>(
                ProjectQueries.Create,
                new
                {
                    Id = project.Id,
                    ExternalId = project.ExternalId,
                    project.Name,
                    project.Description,
                    CustomerExternalId = project.CustomerExternalId,
                    SellerExternalId = project.SellerExternalId,
                    StatusId = (int)project.Status,
                    project.EstimatedAmount,
                    project.StartDateUtc,
                    project.ExpectedCloseDateUtc
                });

            return createdId;
        }
        

        private static int? TryParseStatusId(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            if (!Enum.TryParse<ProjectStatus>(status, ignoreCase: true, out var parsed))
                return null;

            return (int)parsed;
        }

        public async Task<ProjectPagedList> GetAsync(GetProjectsCommand command)
        {
            using var connection = CreateConnection();

            var statusId = TryParseStatusId(command.Status);

            var rows = (await connection.QueryAsync<ProjectSummaryRow>(
                ProjectQueries.Get,
                new
                {
                    CustomerExternalId = string.IsNullOrWhiteSpace(command.CustomerId)
                        ? null
                        : command.CustomerId,

                    SellerExternalId = string.IsNullOrWhiteSpace(command.SellerId)
                        ? null
                        : command.SellerId,

                    StatusId = statusId,
                    Offset = (command.Page - 1) * command.PageSize,
                    PageSize = command.PageSize
                })).ToList();

            var totalItems = rows.FirstOrDefault()?.TotalCount ?? 0;

            return new ProjectPagedList
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

        public async Task<ProjectDetailResult?> GetByExternalIdAsync(GetProjectByExternalIdCommand command)
        {
            using var connection = CreateConnection();

            ProjectDetailRow? row = await connection.QuerySingleOrDefaultAsync<ProjectDetailRow>(
                ProjectQueries.GetByExternalId,
                new { command.ExternalId });

            return row?.ToResult();
        }

        public async Task<ChangeProjectStatusResult> ChangeStatusAsync(ChangeProjectStatusCommand command)
        {
            using var connection = CreateConnection();

            int projectExists = await connection.ExecuteScalarAsync<int>(
                ProjectQueries.ProjectExistsByExternalId,
                new { command.ExternalId });

            if (projectExists == 0)
            {
                return new ChangeProjectStatusResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Proyecto no encontrado."
                };
            }

            int statusExists = await connection.ExecuteScalarAsync<int>(
                ProjectQueries.ProjectStatusExists,
                new { command.StatusId });

            if (statusExists == 0)
            {
                return new ChangeProjectStatusResult
                {
                    Succeeded = false,
                    Message = "Estado de proyecto inválido."
                };
            }

            await connection.ExecuteAsync(
                ProjectQueries.ChangeStatus,
                new
                {
                    command.ExternalId,
                    command.StatusId
                });

            return new ChangeProjectStatusResult
            {
                Succeeded = true,
                Message = "Estado de proyecto actualizado correctamente."
            };
        }
    }
}
