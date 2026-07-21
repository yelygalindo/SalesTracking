using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.ProjectMaterials.Interfaces;
using SalesTracking.Application.UseCases.ProjectMaterials.Results;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectMaterials.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectMaterials;

public sealed class ProjectMaterialsRepository : IProjectMaterialsRepository
{
    private readonly string _connectionString;

    public ProjectMaterialsRepository(IOptions<DatabaseSettings> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<GetProjectMaterialsSummaryResult> GetSummaryAsync(string projectExternalId)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        int projectCount = await connection.ExecuteScalarAsync<int>(
            ProjectMaterialsQueries.ProjectExists,
            new { ProjectExternalId = projectExternalId });

        if (projectCount == 0)
        {
            return new GetProjectMaterialsSummaryResult
            {
                NotFound = true,
                Message = "Proyecto no encontrado."
            };
        }

        IEnumerable<ProjectMaterialSummaryRow> rows = await connection.QueryAsync<ProjectMaterialSummaryRow>(
            ProjectMaterialsQueries.GetSummary,
            new { ProjectExternalId = projectExternalId });

        return new GetProjectMaterialsSummaryResult
        {
            Succeeded = true,
            Items = rows.Select(row => new ProjectMaterialSummaryItemResult
            {
                ProductExternalId = row.ProductExternalId,
                ProductName = row.ProductName,
                Unit = row.Unit,
                CommittedQuantity = row.CommittedQuantity,
                DeliveredQuantity = row.DeliveredQuantity,
                PendingQuantity = row.PendingQuantity
            }).ToList()
        };
    }
}
