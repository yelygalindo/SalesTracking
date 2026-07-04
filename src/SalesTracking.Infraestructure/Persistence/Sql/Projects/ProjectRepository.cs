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

        public async Task<CreateProjectResult> CreateAsync(Project project)
        {
            using var connection = CreateConnection();

            int customerExists = await connection.ExecuteScalarAsync<int>(
                ProjectQueries.CustomerExistsByExternalId,
                new { CustomerExternalId = project.CustomerExternalId });

            if (customerExists == 0)
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "Cliente no encontrado."
                };
            }

            int sellerExists = await connection.ExecuteScalarAsync<int>(
                ProjectQueries.SellerExistsByExternalId,
                new { SellerExternalId = project.SellerExternalId });

            if (sellerExists == 0)
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "Vendedor no encontrado."
                };
            }

            var createdId = await connection.QuerySingleOrDefaultAsync<string>(
                ProjectQueries.Create,
                new
                {
                    ExternalId = project.ExternalId,
                    project.Name,
                    project.Description,
                    CustomerExternalId = project.CustomerExternalId,
                    SellerExternalId = project.SellerExternalId,
                    StatusId = (int)project.Status,
                    project.EstimatedAmount,
                    project.StartDateUtc,
                    project.ExpectedCloseDateUtc,
                    project.ProgressPercentage,
                    project.ActualCloseDateUtc,
                    project.Address,
                    project.Latitude,
                    project.Longitude
                });

            if (string.IsNullOrWhiteSpace(createdId))
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "No se pudo crear el proyecto."
                };
            }

            ProjectDetailRow? createdProject = await connection.QuerySingleOrDefaultAsync<ProjectDetailRow>(
                ProjectQueries.GetByExternalId,
                new { ExternalId = createdId });

            return new CreateProjectResult
            {
                Succeeded = true,
                Id = createdId,
                Message = "Proyecto creado correctamente.",
                Project = createdProject?.ToResult()
            };
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

        public async Task<UpdateProjectResult> UpdateAsync(UpdateProjectCommand command)
        {
            using var connection = CreateConnection();

            int projectExists = await connection.ExecuteScalarAsync<int>(
                ProjectQueries.ProjectExistsByExternalId,
                new { command.ExternalId });

            if (projectExists == 0)
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Proyecto no encontrado."
                };
            }

            int customerExists = await connection.ExecuteScalarAsync<int>(
                ProjectQueries.CustomerExistsByExternalId,
                new { command.CustomerExternalId });

            if (customerExists == 0)
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "Cliente no encontrado."
                };
            }

            int sellerExists = await connection.ExecuteScalarAsync<int>(
                ProjectQueries.SellerExistsByExternalId,
                new { command.SellerExternalId });

            if (sellerExists == 0)
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "Vendedor no encontrado."
                };
            }

            await connection.ExecuteAsync(
                ProjectQueries.Update,
                new
                {
                    command.ExternalId,
                    command.Name,
                    command.Description,
                    command.CustomerExternalId,
                    command.SellerExternalId,
                    command.EstimatedAmount,
                    command.StartDateUtc,
                    command.ExpectedCloseDateUtc,
                    command.ProgressPercentage,
                    command.ActualCloseDateUtc,
                    command.Address,
                    command.Latitude,
                    command.Longitude
                });

            return new UpdateProjectResult
            {
                Succeeded = true,
                Message = "Proyecto actualizado correctamente."
            };
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

        public async Task<DeleteProjectResult> DeleteAsync(DeleteProjectCommand command)
        {
            using var connection = CreateConnection();

            int projectExists = await connection.ExecuteScalarAsync<int>(
                ProjectQueries.ProjectExistsByExternalId,
                new { command.ExternalId });

            if (projectExists == 0)
            {
                return new DeleteProjectResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Proyecto no encontrado."
                };
            }

            await connection.ExecuteAsync(
                ProjectQueries.Delete,
                new { command.ExternalId });

            return new DeleteProjectResult
            {
                Succeeded = true,
                Message = "Proyecto eliminado correctamente."
            };
        }
    }
}
