namespace SalesTracking.Application.UseCases.ProjectAttachments.Comands
{
    public sealed record GetProjectAttachmentContentCommand(string ProjectExternalId, string AttachmentExternalId);
}
