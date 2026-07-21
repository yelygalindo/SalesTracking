namespace SalesTracking.Application.UseCases.ProjectNotes.Comands
{
    public class AddProjectNoteCommand
    {
        public string ProjectExternalId { get; set; }
        public string Content { get; set; }
        public int AuthorUserId { get; set; }
    }
}