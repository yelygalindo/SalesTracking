namespace SalesTracking.Application.UseCases.ProjectNotes.Results
{
    public class UpdateProjectNoteResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; }
    }
}