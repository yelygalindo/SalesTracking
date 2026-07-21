using SalesTracking.Application.UseCases.ProjectAttachments.Comands;
using SalesTracking.Application.UseCases.ProjectAttachments.Results;

namespace SalesTracking.Application.UseCases.ProjectAttachments.Interfaces
{
    public interface IProjectAttachmentService
    {
        Task<GetProjectAttachmentsResult> GetAsync(GetProjectAttachmentsCommand command);
        Task<ProjectAttachmentContentResult> GetContentAsync(GetProjectAttachmentContentCommand command);
        Task<UploadProjectAttachmentResult> UploadAsync(UploadProjectAttachmentCommand command);
        Task<DeleteProjectAttachmentResult> DeleteAsync(DeleteProjectAttachmentCommand command);
        Task<SetProjectAttachmentCoverResult> SetCoverAsync(SetProjectAttachmentCoverCommand command);
    }
}
