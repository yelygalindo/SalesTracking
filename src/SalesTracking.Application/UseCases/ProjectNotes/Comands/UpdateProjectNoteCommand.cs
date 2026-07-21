namespace SalesTracking.Application.UseCases.ProjectNotes.Comands
{
    public class UpdateProjectNoteCommand
    {
        public string ProjectExternalId { get; set; }
        public string NoteExternalId { get; set; }
        public string Content { get; set; }
        public int UpdatedByUserId { get; set; }
    }
}