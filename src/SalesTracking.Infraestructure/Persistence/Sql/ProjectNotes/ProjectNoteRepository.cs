using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.ProjectNotes.Interfaces;
using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using SalesTracking.Application.UseCases.ProjectNotes.Models;
using SalesTracking.Application.UseCases.ProjectNotes.Results;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes.Rows;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes
{
    public sealed class ProjectNoteRepository : IProjectNoteRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly ICurrentUser _currentUser;

        public ProjectNoteRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private int CompanyId => _currentUser.CompanyId;

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<ResponseCreateProjectNote> AddNoteAsync(CreateProjectNote note)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                int? projectInternalId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectNoteQueries.GetProjectInternalIdByExternalId,
                    new { ExternalId = note.ProjectExternalId, CompanyId },
                    transaction);

                if (projectInternalId == null)
                {
                    transaction.Rollback();
                    return new ResponseCreateProjectNote
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Proyecto no encontrado."
                    };
                }

                int noteInternalId = await connection.QuerySingleAsync<int>(
                    ProjectNoteQueries.AddNote,
                    new
                    {
                        note.ExternalId,
                        ProjectId = projectInternalId.Value,
                        note.Content,
                        AuthorId = note.AuthorUserId,
                        CompanyId
                    },
                    transaction);

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = projectInternalId.Value,
                        EventTypeId = ProjectTimelineEventTypeIds.NoteAdded,
                        Title = "Nota agregada",
                        Description = "Nota agregada al proyecto.",
                        CreatedByUserId = note.AuthorUserId,
                        RelatedEntityType = "ProjectNote",
                        RelatedEntityId = noteInternalId
                    });

                transaction.Commit();
                return new ResponseCreateProjectNote
                {
                    Succeeded = true,
                    NotFound = false,
                    Message = "Nota agregada correctamente.",
                    CreateProjectNote = note
                };
            }
            catch
            {
                transaction.Rollback();
                return new ResponseCreateProjectNote
                {
                    Succeeded = false,
                    NotFound = false,
                    Message = "Ocurrio un error al agregar la nota.",
                    CreateProjectNote = note
                };
            }
        }

        public async Task<ResponseUpdateProjectNote> UpdateNoteAsync(UpdateProjectNote note)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                ProjectNoteInternalRow? internalNote = await connection.QueryFirstOrDefaultAsync<ProjectNoteInternalRow>(
                    ProjectNoteQueries.GetInternal,
                    new { note.ProjectExternalId, note.NoteExternalId, CompanyId },
                    transaction);

                if (internalNote == null)
                {
                    transaction.Rollback();
                    return new ResponseUpdateProjectNote
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Nota de proyecto no encontrada."
                    };
                }

                int affectedRows = await connection.ExecuteAsync(
                    ProjectNoteQueries.UpdateNote,
                    new
                    {
                        note.ProjectExternalId,
                        note.NoteExternalId,
                        note.Content,
                        UpdatedByUserId = note.UpdatedByUserId,
                        CompanyId
                    },
                    transaction);

                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    return new ResponseUpdateProjectNote
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Nota de proyecto no encontrada."
                    };
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = internalNote.ProjectId,
                        EventTypeId = ProjectTimelineEventTypeIds.NoteUpdated,
                        Title = "Nota actualizada",
                        Description = "Nota del proyecto actualizada.",
                        CreatedByUserId = note.UpdatedByUserId,
                        RelatedEntityType = "ProjectNote",
                        RelatedEntityId = internalNote.Id
                    });

                transaction.Commit();
                return new ResponseUpdateProjectNote
                {
                    Succeeded = true,
                    NotFound = false,
                    Message = "Nota actualizada correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
                return new ResponseUpdateProjectNote
                {
                    Succeeded = false,
                    NotFound = false,
                    Message = "Ocurrio un error al actualizar la nota."
                };
            }
        }

        public async Task<ResponseDeleteProjectNote> DeleteNoteAsync(DeleteProjectNoteCommand command)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                ProjectNoteInternalRow? internalNote = await connection.QueryFirstOrDefaultAsync<ProjectNoteInternalRow>(
                    ProjectNoteQueries.GetInternal,
                    new
                    {
                        command.ProjectExternalId,
                        command.NoteExternalId,
                        CompanyId
                    },
                    transaction);

                if (internalNote == null)
                {
                    transaction.Rollback();
                    return new ResponseDeleteProjectNote
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Nota de proyecto no encontrada."
                    };
                }

                int affectedRows = await connection.ExecuteAsync(
                    ProjectNoteQueries.DeleteNote,
                    new
                    {
                        command.ProjectExternalId,
                        command.NoteExternalId,
                        CompanyId
                    },
                    transaction);

                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    return new ResponseDeleteProjectNote
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Nota de proyecto no encontrada."
                    };
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = internalNote.ProjectId,
                        EventTypeId = ProjectTimelineEventTypeIds.NoteDeleted,
                        Title = "Nota eliminada",
                        Description = "Nota eliminada del proyecto.",
                        CreatedByUserId = command.DeletedByUserId,
                        RelatedEntityType = "ProjectNote",
                        RelatedEntityId = internalNote.Id
                    });

                transaction.Commit();

                return new ResponseDeleteProjectNote
                {
                    Succeeded = true,
                    NotFound = false,
                    Message = "Nota eliminada correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
                return new ResponseDeleteProjectNote
                {
                    Succeeded = false,
                    NotFound = false,
                    Message = "Ocurrio un error al eliminar la nota."
                };
            }
        }

        public async Task<ProjectNoteResult?> GetNoteAsync(string projectExternalId, string noteExternalId)
        {
            using IDbConnection connection = CreateConnection();

            ProjectNoteRow? row = await connection.QueryFirstOrDefaultAsync<ProjectNoteRow>(
                ProjectNoteQueries.GetByExternalId,
                new
                {
                    ProjectExternalId = projectExternalId,
                    NoteExternalId = noteExternalId,
                    CompanyId
                });

            return row?.ToResult();
        }

        public async Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(string projectExternalId)
        {
            using IDbConnection connection = CreateConnection();

            IEnumerable<ProjectNoteRow> rows = await connection.QueryAsync<ProjectNoteRow>(
                ProjectNoteQueries.GetByProjectExternalId,
                new { ProjectExternalId = projectExternalId, CompanyId });

            return rows.Select(x => x.ToResult()).ToList();
        }

        private sealed class ProjectNoteInternalRow
        {
            public int Id { get; set; }
            public int ProjectId { get; set; }
        }
    }
}
