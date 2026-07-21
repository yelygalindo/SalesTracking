using SalesTracking.Application.UseCases.ProjectAttachments.Comands;
using SalesTracking.Application.UseCases.ProjectAttachments.Models;
using SalesTracking.Application.UseCases.ProjectAttachments.Results;

namespace SalesTracking.Application.UseCases.ProjectAttachments.Interfaces
{
    public interface IProjectAttachmentRepository
    {
        Task<GetProjectAttachmentsResult> GetAsync(string projectExternalId);
        Task<ProjectAttachmentContentResult> GetContentInfoAsync(string projectExternalId, string attachmentExternalId);
        Task<UploadProjectAttachmentResult> CreateAsync(CreateProjectAttachment attachment);
        Task<DeleteProjectAttachmentResult> DeleteAsync(DeleteProjectAttachmentCommand command);
        Task<SetProjectAttachmentCoverResult> SetCoverAsync(SetProjectAttachmentCoverCommand command);
    }
}
