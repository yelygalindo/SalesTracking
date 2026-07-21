using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.ProjectAttachments.Comands;
using SalesTracking.Application.UseCases.ProjectAttachments.Interfaces;
using SalesTracking.Application.UseCases.ProjectAttachments.Models;
using SalesTracking.Application.UseCases.ProjectAttachments.Results;
using SalesTracking.Application.Common.Interfaces;
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
        private readonly ICurrentUser _currentUser;

        public ProjectAttachmentRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private int CompanyId => _currentUser.CompanyId.GetValueOrDefault();

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<GetProjectAttachmentsResult> GetAsync(string projectExternalId)
        {
            using IDbConnection connection = CreateConnection();

            bool projectExists = await connection.QuerySingleAsync<int>(
                ProjectAttachmentQueries.ProjectExists,
                new { ProjectExternalId = projectExternalId, CompanyId }) > 0;

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
                new { ProjectExternalId = projectExternalId, CompanyId });

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
                    AttachmentExternalId = attachmentExternalId,
                    CompanyId
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
                    new { ExternalId = attachment.ProjectExternalId, CompanyId },
                    transaction);

                if (projectId == null)
                    return RollbackUpload(transaction, "Proyecto no encontrado.", true);

                if (attachment.IsCover)
                {
                    await connection.ExecuteAsync(
                        ProjectAttachmentQueries.ClearCover,
                        new
                        {
                            ProjectId = projectId.Value,
                            UpdatedByUserId = attachment.UploadedByUserId,
                            CompanyId
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
                        UploadedByUserId = attachment.UploadedByUserId,
                        CompanyId
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
                        CreatedByUserId = attachment.UploadedByUserId,
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
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                ProjectAttachmentInternalRow? attachment = await connection.QueryFirstOrDefaultAsync<ProjectAttachmentInternalRow>(
                    ProjectAttachmentQueries.GetAttachmentInternal,
                    new { command.ProjectExternalId, command.AttachmentExternalId, CompanyId },
                    transaction);

                int affectedRows = await connection.ExecuteAsync(
                    ProjectAttachmentQueries.Delete,
                    new
                    {
                        command.ProjectExternalId,
                        command.AttachmentExternalId,
                        DeletedByUserId = command.DeletedByUserId,
                        CompanyId
                    },
                    transaction);

                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    return new DeleteProjectAttachmentResult
                    {
                        Succeeded = false,
                        NotFound = true,
                        Message = "Archivo no encontrado."
                    };
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = attachment!.ProjectId,
                        EventTypeId = ProjectTimelineEventTypeIds.AttachmentDeleted,
                        Title = "Archivo eliminado",
                        Description = "Archivo eliminado del proyecto.",
                        CreatedByUserId = command.DeletedByUserId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = attachment.Id
                    });

                transaction.Commit();

                return new DeleteProjectAttachmentResult
                {
                    Succeeded = true,
                    Message = "Archivo eliminado correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
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

                ProjectAttachmentInternalRow? attachment = await connection.QueryFirstOrDefaultAsync<ProjectAttachmentInternalRow>(
                    ProjectAttachmentQueries.GetAttachmentInternal,
                    new
                    {
                        command.ProjectExternalId,
                        command.AttachmentExternalId,
                        CompanyId
                    },
                    transaction);

                if (attachment == null)
                    return RollbackSetCover(transaction, "Archivo no encontrado.", true);

                await connection.ExecuteAsync(
                    ProjectAttachmentQueries.ClearCover,
                    new
                    {
                        attachment.ProjectId,
                        UpdatedByUserId = command.UpdatedByUserId,
                        CompanyId
                    },
                    transaction);

                int affectedRows = await connection.ExecuteAsync(
                    ProjectAttachmentQueries.SetCover,
                    new
                    {
                        AttachmentId = attachment.Id,
                        attachment.ProjectId,
                        UpdatedByUserId = command.UpdatedByUserId,
                        CompanyId
                    },
                    transaction);

                if (affectedRows == 0)
                    return RollbackSetCover(transaction, "Archivo no encontrado.", true);

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = attachment.ProjectId,
                        EventTypeId = ProjectTimelineEventTypeIds.AttachmentCoverChanged,
                        Title = "Portada actualizada",
                        Description = "Se selecciono un archivo como portada del proyecto.",
                        CreatedByUserId = command.UpdatedByUserId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = attachment.Id
                    });

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
