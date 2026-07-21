namespace SalesTracking.Application.UseCases.ProjectAttachments.Comands
{
    public sealed record SetProjectAttachmentCoverCommand(
        string ProjectExternalId,
        string AttachmentExternalId,
        int UpdatedByUserId);
}