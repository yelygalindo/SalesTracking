namespace SalesTracking.Application.UseCases.ProjectNotes.Models
{
    public class CreateProjectNote
    {
        public string ExternalId { get; set; }
        public string ProjectExternalId { get; set; }
        public string Content { get; set; }
        public int AuthorUserId { get; set; }
    }
}