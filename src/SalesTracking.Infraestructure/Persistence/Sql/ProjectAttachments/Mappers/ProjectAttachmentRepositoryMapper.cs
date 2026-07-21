using SalesTracking.Application.UseCases.ProjectAttachments.Results;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments.Mappers
{
    internal static class ProjectAttachmentRepositoryMapper
    {
        public static ProjectAttachmentResult ToResult(this ProjectAttachmentRow row)
        {
            return new ProjectAttachmentResult
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                FileName = row.FileName,
                ContentType = row.ContentType,
                SizeBytes = row.SizeBytes,
                AttachmentType = row.AttachmentType,
                Caption = row.Caption,
                IsCover = row.IsCover,
                UploadedByUserExternalId = row.UploadedByUserExternalId,
                UploadedByUserName = row.UploadedByUserName,
                CreatedAtUtc = row.CreatedAtUtc,
                UpdatedAtUtc = row.UpdatedAtUtc
            };
        }
    }
}
