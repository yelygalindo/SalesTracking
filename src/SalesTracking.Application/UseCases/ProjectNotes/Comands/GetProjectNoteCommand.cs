namespace SalesTracking.Application.UseCases.ProjectNotes.Comands
{
    public sealed record GetProjectNoteCommand(string ProjectExternalId, string NoteExternalId);
}