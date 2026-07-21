using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.ProjectNotes.Interfaces;
using SalesTracking.Application.UseCases.ProjectNotes.Models;
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

        public async Task<ResponseCreateProjectNote> AddNoteAsync(CreateProjectNote note)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                int? projectInternalId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectNoteQueries.GetProjectInternalIdByExternalId,
                    new { ExternalId = note.ProjectExternalId },
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

                int? authorInternalId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectNoteQueries.GetUserInternalIdByExternalId,
                    new { ExternalId = note.AuthorExternalId },
                    transaction);

                if (authorInternalId == null)
                {
                    transaction.Rollback();
                    return new ResponseCreateProjectNote
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Autor no encontrado o inactivo."
                    };
                }

                await connection.ExecuteAsync(
                    ProjectNoteQueries.AddNote,
                    new
                    {
                        note.ExternalId,
                        ProjectId = projectInternalId.Value,
                        note.Content,
                        AuthorId = authorInternalId.Value
                    },
                    transaction);

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
                int? updatedByUserId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectNoteQueries.GetUserInternalIdByExternalId,
                    new { ExternalId = note.UpdatedByUserExternalId },
                    transaction);

                if (updatedByUserId == null)
                {
                    transaction.Rollback();
                    return new ResponseUpdateProjectNote
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Usuario no encontrado o inactivo."
                    };
                }

                int affectedRows = await connection.ExecuteAsync(
                    ProjectNoteQueries.UpdateNote,
                    new
                    {
                        note.ProjectExternalId,
                        note.NoteExternalId,
                        note.Content,
                        UpdatedByUserId = updatedByUserId.Value
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

        public async Task<ResponseDeleteProjectNote> DeleteNoteAsync(string projectExternalId, string noteExternalId)
        {
            using IDbConnection connection = CreateConnection();

            try
            {
                int affectedRows = await connection.ExecuteAsync(
                    ProjectNoteQueries.DeleteNote,
                    new
                    {
                        ProjectExternalId = projectExternalId,
                        NoteExternalId = noteExternalId
                    });

                if (affectedRows == 0)
                {
                    return new ResponseDeleteProjectNote
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Nota de proyecto no encontrada."
                    };
                }

                return new ResponseDeleteProjectNote
                {
                    Succeeded = true,
                    NotFound = false,
                    Message = "Nota eliminada correctamente."
                };
            }
            catch
            {
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
                    NoteExternalId = noteExternalId
                });

            return row?.ToResult();
        }

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