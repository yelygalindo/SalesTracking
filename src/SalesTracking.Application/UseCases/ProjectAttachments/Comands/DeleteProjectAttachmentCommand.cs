namespace SalesTracking.Application.UseCases.ProjectAttachments.Comands
{
    public sealed record DeleteProjectAttachmentCommand(
        string ProjectExternalId,
        string AttachmentExternalId,
        string DeletedByUserExternalId);
}
