using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.ProjectNotes.Interfaces;
using SalesTracking.Application.UseCases.ProjectNotes.Results;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes
{
    public sealed class ProjectNoteRepository : IProjectNoteRepository
    {
        private readonly DatabaseSettings _databaseOptions;

        public ProjectNoteRepository(IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(string projectExternalId)
        {
            using IDbConnection connection = CreateConnection();

            IEnumerable<ProjectNoteRow> rows = await connection.QueryAsync<ProjectNoteRow>(
                ProjectNoteQueries.GetByProjectExternalId,
                new { ProjectExternalId = projectExternalId });

            return rows.Select(x => x.ToResult()).ToList();
        }
    }
}
