using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.ProjectVisits.Comands;
using SalesTracking.Application.UseCases.ProjectVisits.Interfaces;
using SalesTracking.Application.UseCases.ProjectVisits.Models;
using SalesTracking.Application.UseCases.ProjectVisits.Results;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectVisits.Rows;
using System.Data;
using System.Text.Json;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectVisits;

public sealed class ProjectVisitRepository : IProjectVisitRepository
{
    private readonly string _connectionString;
    private readonly ICurrentUser _currentUser;

    public ProjectVisitRepository(IOptions<DatabaseSettings> options, ICurrentUser currentUser)
    {
        _connectionString = options.Value.ConnectionString;
        _currentUser = currentUser;
    }

    private int CompanyId => _currentUser.CompanyId.GetValueOrDefault();

    public async Task<CreateProjectVisitResult> CreateAsync(CreateProjectVisit visit)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbTransaction transaction = connection.BeginTransaction();

        try
        {
            int? projectId = await connection.QueryFirstOrDefaultAsync<int?>(
                ProjectVisitQueries.GetProjectId,
                new { visit.ProjectExternalId, CompanyId },
                transaction);

            if (!projectId.HasValue)
            {
                transaction.Rollback();
                return new CreateProjectVisitResult { NotFound = true, Message = "Proyecto no encontrado." };
            }

            string metadataJson = JsonSerializer.Serialize(new
            {
                externalId = visit.ExternalId,
                latitude = visit.Latitude,
                longitude = visit.Longitude,
                notes = visit.Notes,
                result = visit.Result
            });

            await ProjectTimelineWriter.InsertAsync(connection, transaction, new ProjectTimelineEvent
            {
                ProjectId = projectId.Value,
                EventTypeId = ProjectTimelineEventTypeIds.ProjectVisited,
                Title = "Visita a obra registrada",
                Description = visit.Notes,
                OccurredAtUtc = visit.VisitedAtUtc,
                CreatedByUserId = visit.SellerUserId,
                RelatedEntityType = "ProjectVisit",
                MetadataJson = metadataJson
            });

            transaction.Commit();
            return new CreateProjectVisitResult
            {
                Succeeded = true,
                Id = visit.ExternalId,
                Message = "Visita registrada correctamente."
            };
        }
        catch
        {
            transaction.Rollback();
            return new CreateProjectVisitResult { Message = "No se pudo registrar la visita." };
        }
    }

    public async Task<GetProjectVisitsResult> GetAsync(GetProjectVisitsCommand command)
    {
        using IDbConnection connection = CreateConnection();
        int? projectId = await connection.QueryFirstOrDefaultAsync<int?>(
            ProjectVisitQueries.GetProjectId,
            new { command.ProjectExternalId, CompanyId });

        if (!projectId.HasValue)
            return new GetProjectVisitsResult { NotFound = true, Message = "Proyecto no encontrado." };

        IEnumerable<ProjectVisitRow> rows = await connection.QueryAsync<ProjectVisitRow>(
            ProjectVisitQueries.Get,
            new
            {
                ProjectId = projectId.Value,
                command.SellerExternalId,
                FromUtc = command.From?.UtcDateTime,
                ToUtc = command.To?.UtcDateTime,
                CompanyId
            });

        return new GetProjectVisitsResult
        {
            Succeeded = true,
            Visits = rows.Select(row => new ProjectVisitResult
            {
                ExternalId = row.ExternalId,
                VisitedAtUtc = DateTime.SpecifyKind(row.VisitedAtUtc, DateTimeKind.Utc),
                Latitude = row.Latitude,
                Longitude = row.Longitude,
                Notes = row.Notes,
                Result = row.Result,
                SellerExternalId = row.SellerExternalId,
                SellerName = row.SellerName
            }).ToList()
        };
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
