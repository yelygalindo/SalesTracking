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
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline;
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

        public async Task<CreateProjectResult> CreateAsync(Project project, int createdByUserId)
        {
            using var connection = CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                int customerExists = await connection.ExecuteScalarAsync<int>(
                    ProjectQueries.CustomerExistsByExternalId,
                    new { CustomerExternalId = project.CustomerExternalId },
                    transaction);

                if (customerExists == 0)
                {
                    transaction.Rollback();
                    return new CreateProjectResult
                    {
                        Succeeded = false,
                        Message = "Cliente no encontrado."
                    };
                }

                int sellerExists = await connection.ExecuteScalarAsync<int>(
                    ProjectQueries.SellerExistsByExternalId,
                    new { SellerExternalId = project.SellerExternalId },
                    transaction);

                if (sellerExists == 0)
                {
                    transaction.Rollback();
                    return new CreateProjectResult
                    {
                        Succeeded = false,
                        Message = "Vendedor no encontrado."
                    };
                }

                string? createdId = await connection.QuerySingleOrDefaultAsync<string>(
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
                    },
                    transaction);

                if (string.IsNullOrWhiteSpace(createdId))
                {
                    transaction.Rollback();
                    return new CreateProjectResult
                    {
                        Succeeded = false,
                        Message = "No se pudo crear el proyecto."
                    };
                }

                ProjectTimelineProjectRow? createdProjectInfo = await connection.QuerySingleOrDefaultAsync<ProjectTimelineProjectRow>(
                    ProjectQueries.GetTimelineProjectByExternalId,
                    new { ExternalId = createdId },
                    transaction);

                if (createdProjectInfo == null)
                {
                    transaction.Rollback();
                    return new CreateProjectResult
                    {
                        Succeeded = false,
                        Message = "No se pudo obtener el proyecto creado."
                    };
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = createdProjectInfo.Id,
                        EventTypeId = ProjectTimelineEventTypeIds.ProjectCreated,
                        Title = "Proyecto creado",
                        Description = "Proyecto creado correctamente.",
                        CreatedByUserId = createdByUserId
                    });

                ProjectDetailRow? createdProject = await connection.QuerySingleOrDefaultAsync<ProjectDetailRow>(
                    ProjectQueries.GetByExternalId,
                    new { ExternalId = createdId },
                    transaction);

                transaction.Commit();
                return new CreateProjectResult
                {
                    Succeeded = true,
                    Id = createdId,
                    Message = "Proyecto creado correctamente.",
                    Project = createdProject?.ToResult()
                };
            }
            catch
            {
                transaction.Rollback();
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al crear el proyecto."
                };
            }
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
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                ProjectTimelineProjectRow? projectInfo = await connection.QuerySingleOrDefaultAsync<ProjectTimelineProjectRow>(
                    ProjectQueries.GetTimelineProjectByExternalId,
                    new { command.ExternalId },
                    transaction);

                if (projectInfo == null)
                {
                    transaction.Rollback();
                    return new UpdateProjectResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Proyecto no encontrado."
                    };
                }

                int customerExists = await connection.ExecuteScalarAsync<int>(
                    ProjectQueries.CustomerExistsByExternalId,
                    new { command.CustomerExternalId },
                    transaction);

                if (customerExists == 0)
                {
                    transaction.Rollback();
                    return new UpdateProjectResult
                    {
                        Succeeded = false,
                        Message = "Cliente no encontrado."
                    };
                }

                int? sellerInternalId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectQueries.GetUserInternalIdByExternalId,
                    new { ExternalId = command.SellerExternalId },
                    transaction);

                if (sellerInternalId == null)
                {
                    transaction.Rollback();
                    return new UpdateProjectResult
                    {
                        Succeeded = false,
                        Message = "Vendedor no encontrado."
                    };
                }

                int affectedRows = await connection.ExecuteAsync(
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
                    },
                    transaction);

                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    return new UpdateProjectResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Proyecto no encontrado."
                    };
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = projectInfo.Id,
                        EventTypeId = ProjectTimelineEventTypeIds.ProjectUpdated,
                        Title = "Proyecto actualizado",
                        Description = "Proyecto actualizado correctamente.",
                        CreatedByUserId = command.UpdatedByUserId
                    });

                decimal newProgress = command.ProgressPercentage ?? 0;
                if (projectInfo.ProgressPercentage != newProgress)
                {
                    await ProjectTimelineWriter.InsertAsync(
                        connection,
                        transaction,
                        new ProjectTimelineEvent
                        {
                            ProjectId = projectInfo.Id,
                            EventTypeId = ProjectTimelineEventTypeIds.ProjectProgressUpdated,
                            Title = "Avance actualizado",
                            Description = $"Porcentaje de avance actualizado de {projectInfo.ProgressPercentage} a {newProgress}.",
                            CreatedByUserId = command.UpdatedByUserId
                        });
                }

                transaction.Commit();
                return new UpdateProjectResult
                {
                    Succeeded = true,
                    Message = "Proyecto actualizado correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al actualizar el proyecto."
                };
            }
        }

        public async Task<ChangeProjectStatusResult> ChangeStatusAsync(ChangeProjectStatusCommand command)
        {
            using var connection = CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                ProjectTimelineProjectRow? projectInfo = await connection.QuerySingleOrDefaultAsync<ProjectTimelineProjectRow>(
                    ProjectQueries.GetTimelineProjectByExternalId,
                    new { command.ExternalId },
                    transaction);

                if (projectInfo == null)
                {
                    transaction.Rollback();
                    return new ChangeProjectStatusResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Proyecto no encontrado."
                    };
                }

                int statusExists = await connection.ExecuteScalarAsync<int>(
                    ProjectQueries.ProjectStatusExists,
                    new { command.StatusId },
                    transaction);

                if (statusExists == 0)
                {
                    transaction.Rollback();
                    return new ChangeProjectStatusResult
                    {
                        Succeeded = false,
                        Message = "Estado de proyecto invalido."
                    };
                }

                int affectedRows = await connection.ExecuteAsync(
                    ProjectQueries.ChangeStatus,
                    new
                    {
                        command.ExternalId,
                        command.StatusId
                    },
                    transaction);

                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    return new ChangeProjectStatusResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Proyecto no encontrado."
                    };
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = projectInfo.Id,
                        EventTypeId = ProjectTimelineEventTypeIds.ProjectStatusChanged,
                        Title = "Estado actualizado",
                        Description = $"Estado de proyecto actualizado de {projectInfo.StatusId} a {command.StatusId}.",
                        CreatedByUserId = command.ChangedByUserId
                    });

                transaction.Commit();
                return new ChangeProjectStatusResult
                {
                    Succeeded = true,
                    Message = "Estado de proyecto actualizado correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
                return new ChangeProjectStatusResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al actualizar el estado del proyecto."
                };
            }
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

        private sealed class ProjectTimelineProjectRow
        {
            public int Id { get; set; }
            public int SellerId { get; set; }
            public int StatusId { get; set; }
            public decimal ProgressPercentage { get; set; }
        }
    }
}