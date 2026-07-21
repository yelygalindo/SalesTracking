namespace SalesTracking.Application.UseCases.ProjectNotes.Results
{
    public class AddProjectNoteResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Id { get; set; }
        public string Message { get; set; }
    }
}
