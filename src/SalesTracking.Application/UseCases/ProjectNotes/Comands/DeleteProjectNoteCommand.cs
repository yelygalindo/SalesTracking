namespace SalesTracking.Application.UseCases.ProjectNotes.Comands
{
    public sealed record DeleteProjectNoteCommand(string ProjectExternalId, string NoteExternalId);
}