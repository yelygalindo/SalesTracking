using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.ProjectTimeline.Comands;
using SalesTracking.Application.UseCases.ProjectTimeline.Interfaces;
using SalesTracking.Application.UseCases.ProjectTimeline.Models;
using SalesTracking.Application.UseCases.ProjectTimeline.Results;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline
{
    public sealed class ProjectTimelineRepository : IProjectTimelineRepository
    {
        private readonly DatabaseSettings _databaseOptions;

        public ProjectTimelineRepository(IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<GetProjectTimelineResult> GetAsync(GetProjectTimelineCommand command)
        {
            using IDbConnection connection = CreateConnection();

            int? projectId = await connection.QueryFirstOrDefaultAsync<int?>(
                ProjectTimelineQueries.GetProjectInternalIdByExternalId,
                new { command.ProjectExternalId });

            if (projectId == null)
            {
                return new GetProjectTimelineResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Proyecto no encontrado."
                };
            }

            var rows = (await connection.QueryAsync<ProjectTimelineRow>(
                ProjectTimelineQueries.GetByProjectId,
                new
                {
                    ProjectId = projectId.Value,
                    Offset = (command.Page - 1) * command.PageSize,
                    command.PageSize
                })).ToList();

            int totalItems = rows.FirstOrDefault()?.TotalCount ?? 0;

            return new GetProjectTimelineResult
            {
                Succeeded = true,
                Timeline = new ProjectTimelinePagedList
                {
                    Items = rows.Select(x => x.ToResult()).ToList(),
                    Page = command.Page,
                    PageSize = command.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalItems == 0
                        ? 0
                        : (int)Math.Ceiling(totalItems / (double)command.PageSize)
                }
            };
        }
    }
}