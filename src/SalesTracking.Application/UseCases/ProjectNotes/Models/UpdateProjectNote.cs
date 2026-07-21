namespace SalesTracking.Application.UseCases.ProjectNotes.Models
{
    public class UpdateProjectNote
    {
        public string ProjectExternalId { get; set; }
        public string NoteExternalId { get; set; }
        public string Content { get; set; }
        public string UpdatedByUserExternalId { get; set; }
    }
}