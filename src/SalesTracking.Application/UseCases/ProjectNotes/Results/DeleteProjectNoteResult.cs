namespace SalesTracking.Application.UseCases.ProjectNotes.Results
{
    public class DeleteProjectNoteResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; }
    }
}