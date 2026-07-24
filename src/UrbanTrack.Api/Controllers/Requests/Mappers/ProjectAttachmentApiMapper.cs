using SalesTracking.Application.UseCases.ProjectAttachments.Comands;
using SalesTracking.Application.UseCases.ProjectAttachments.Results;
using UrbanTrack.Api.Controllers.Requests.ProjectAttachments;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.ProjectAttachments;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class ProjectAttachmentApiMapper
    {
        public static UploadProjectAttachmentCommand ToApplication(
            this UploadProjectAttachmentRequest request,
            string projectExternalId,
            int uploadedByUserId)
        {
            return new UploadProjectAttachmentCommand
            {
                ProjectExternalId = projectExternalId,
                FileName = request.File?.FileName ?? string.Empty,
                ContentType = request.File?.ContentType ?? string.Empty,
                SizeBytes = request.File?.Length ?? 0,
                Content = request.File?.OpenReadStream() ?? Stream.Null,
                AttachmentType = request.AttachmentType ?? string.Empty,
                Caption = request.Caption,
                IsCover = request.IsCover ?? false,
                UploadedByUserId = uploadedByUserId
            };
        }

        public static ProjectAttachmentResponse ToResponse(this ProjectAttachmentResult result)
        {
            return new ProjectAttachmentResponse
            {
                Id = result.Id,
                ExternalId = result.ExternalId,
                FileName = result.FileName,
                ContentType = result.ContentType,
                SizeBytes = result.SizeBytes,
                AttachmentType = result.AttachmentType,
                Caption = result.Caption,
                IsCover = result.IsCover,
                UploadedByUserExternalId = result.UploadedByUserExternalId,
                UploadedByUserName = result.UploadedByUserName,
                CreatedAtUtc = result.CreatedAtUtc,
                UpdatedAtUtc = result.UpdatedAtUtc
            };
        }

        public static IdMessageResponse ToResponse(this UploadProjectAttachmentResult result)
        {
            return new IdMessageResponse
            {
                Id = result.Id,
                Message = result.Message
            };
        }
    }
}
