using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.ProjectAttachments.Comands;
using SalesTracking.Application.UseCases.ProjectAttachments.Interfaces;
using SalesTracking.Application.UseCases.ProjectAttachments.Models;
using SalesTracking.Application.UseCases.ProjectAttachments.Results;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments.Rows;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline;
using System.Data;
using System.Text.Json;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments
{
    public sealed class ProjectAttachmentRepository : IProjectAttachmentRepository
    {
        private const string RelatedEntityType = "ProjectAttachment";
        private readonly DatabaseSettings _databaseOptions;

        public ProjectAttachmentRepository(IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<GetProjectAttachmentsResult> GetAsync(string projectExternalId)
        {
            using IDbConnection connection = CreateConnection();

            bool projectExists = await connection.QuerySingleAsync<int>(
                ProjectAttachmentQueries.ProjectExists,
                new { ProjectExternalId = projectExternalId }) > 0;

            if (!projectExists)
            {
                return new GetProjectAttachmentsResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Proyecto no encontrado."
                };
            }

            var rows = await connection.QueryAsync<ProjectAttachmentRow>(
                ProjectAttachmentQueries.Get,
                new { ProjectExternalId = projectExternalId });

            return new GetProjectAttachmentsResult
            {
                Succeeded = true,
                Items = rows.Select(x => x.ToResult()).ToList()
            };
        }

        public async Task<ProjectAttachmentContentResult> GetContentInfoAsync(
            string projectExternalId,
            string attachmentExternalId)
        {
            using IDbConnection connection = CreateConnection();

            ProjectAttachmentContentRow? row = await connection.QueryFirstOrDefaultAsync<ProjectAttachmentContentRow>(
                ProjectAttachmentQueries.GetContentInfo,
                new
                {
                    ProjectExternalId = projectExternalId,
                    AttachmentExternalId = attachmentExternalId
                });

            if (row == null)
            {
                return new ProjectAttachmentContentResult
                {
                    Succeeded = false,
                    NotFound = true,
                    Message = "Archivo no encontrado."
                };
            }

            return new ProjectAttachmentContentResult
            {
                Succeeded = true,
                FileName = row.FileName,
                ContentType = row.ContentType,
                StorageKey = row.StorageKey
            };
        }

        public async Task<UploadProjectAttachmentResult> CreateAsync(CreateProjectAttachment attachment)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                int? projectId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectAttachmentQueries.GetProjectInternalIdByExternalId,
                    new { ExternalId = attachment.ProjectExternalId },
                    transaction);

                if (projectId == null)
                    return RollbackUpload(transaction, "Proyecto no encontrado.", true);

                int? uploadedByUserId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectAttachmentQueries.GetUserInternalIdByExternalId,
                    new { ExternalId = attachment.UploadedByUserExternalId },
                    transaction);

                if (uploadedByUserId == null)
                    return RollbackUpload(transaction, "Usuario no encontrado o inactivo.", true);

                if (attachment.IsCover)
                {
                    await connection.ExecuteAsync(
                        ProjectAttachmentQueries.ClearCover,
                        new
                        {
                            ProjectId = projectId.Value,
                            UpdatedByUserId = uploadedByUserId.Value
                        },
                        transaction);
                }

                int attachmentId = await connection.QuerySingleAsync<int>(
                    ProjectAttachmentQueries.Insert,
                    new
                    {
                        attachment.ExternalId,
                        ProjectId = projectId.Value,
                        attachment.FileName,
                        attachment.StorageProvider,
                        attachment.StorageKey,
                        attachment.ContentType,
                        attachment.SizeBytes,
                        attachment.AttachmentType,
                        attachment.Caption,
                        attachment.IsCover,
                        UploadedByUserId = uploadedByUserId.Value
                    },
                    transaction);

                string metadataJson = JsonSerializer.Serialize(new
                {
                    attachmentExternalId = attachment.ExternalId,
                    fileName = attachment.FileName,
                    attachmentType = attachment.AttachmentType,
                    contentType = attachment.ContentType,
                    sizeBytes = attachment.SizeBytes
                });

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = projectId.Value,
                        EventTypeId = ProjectTimelineEventTypeIds.AttachmentUploaded,
                        Title = "Archivo agregado",
                        Description = "Archivo agregado al proyecto.",
                        CreatedByUserId = uploadedByUserId.Value,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = attachmentId,
                        MetadataJson = metadataJson
                    });

                transaction.Commit();

                return new UploadProjectAttachmentResult
                {
                    Succeeded = true,
                    Id = attachment.ExternalId,
                    Message = "Archivo agregado correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
                return new UploadProjectAttachmentResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al agregar el archivo."
                };
            }
        }

        public async Task<DeleteProjectAttachmentResult> DeleteAsync(DeleteProjectAttachmentCommand command)
        {
            using IDbConnection connection = CreateConnection();

            try
            {
                int? deletedByUserId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectAttachmentQueries.GetUserInternalIdByExternalId,
                    new { ExternalId = command.DeletedByUserExternalId });

                if (deletedByUserId == null)
                {
                    return new DeleteProjectAttachmentResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Usuario no encontrado o inactivo."
                    };
                }

                int affectedRows = await connection.ExecuteAsync(
                    ProjectAttachmentQueries.Delete,
                    new
                    {
                        command.ProjectExternalId,
                        command.AttachmentExternalId,
                        DeletedByUserId = deletedByUserId.Value
                    });

                if (affectedRows == 0)
                {
                    return new DeleteProjectAttachmentResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Archivo no encontrado."
                    };
                }

                return new DeleteProjectAttachmentResult
                {
                    Succeeded = true,
                    Message = "Archivo eliminado correctamente."
                };
            }
            catch
            {
                return new DeleteProjectAttachmentResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al eliminar el archivo."
                };
            }
        }

        public async Task<SetProjectAttachmentCoverResult> SetCoverAsync(SetProjectAttachmentCoverCommand command)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                int? updatedByUserId = await connection.QueryFirstOrDefaultAsync<int?>(
                    ProjectAttachmentQueries.GetUserInternalIdByExternalId,
                    new { ExternalId = command.UpdatedByUserExternalId },
                    transaction);

                if (updatedByUserId == null)
                    return RollbackSetCover(transaction, "Usuario no encontrado o inactivo.", true);

                ProjectAttachmentInternalRow? attachment = await connection.QueryFirstOrDefaultAsync<ProjectAttachmentInternalRow>(
                    ProjectAttachmentQueries.GetAttachmentInternal,
                    new
                    {
                        command.ProjectExternalId,
                        command.AttachmentExternalId
                    },
                    transaction);

                if (attachment == null)
                    return RollbackSetCover(transaction, "Archivo no encontrado.", true);

                await connection.ExecuteAsync(
                    ProjectAttachmentQueries.ClearCover,
                    new
                    {
                        attachment.ProjectId,
                        UpdatedByUserId = updatedByUserId.Value
                    },
                    transaction);

                int affectedRows = await connection.ExecuteAsync(
                    ProjectAttachmentQueries.SetCover,
                    new
                    {
                        AttachmentId = attachment.Id,
                        attachment.ProjectId,
                        UpdatedByUserId = updatedByUserId.Value
                    },
                    transaction);

                if (affectedRows == 0)
                    return RollbackSetCover(transaction, "Archivo no encontrado.", true);

                transaction.Commit();
                return new SetProjectAttachmentCoverResult
                {
                    Succeeded = true,
                    Message = "Portada actualizada correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
                return new SetProjectAttachmentCoverResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al actualizar la portada."
                };
            }
        }

        private static UploadProjectAttachmentResult RollbackUpload(IDbTransaction transaction, string message, bool notFound)
        {
            transaction.Rollback();
            return new UploadProjectAttachmentResult
            {
                Succeeded = false,
                NotFound = notFound,
                Message = message
            };
        }

        private static SetProjectAttachmentCoverResult RollbackSetCover(IDbTransaction transaction, string message, bool notFound)
        {
            transaction.Rollback();
            return new SetProjectAttachmentCoverResult
            {
                Succeeded = false,
                NotFound = notFound,
                Message = message
            };
        }
    }
}
