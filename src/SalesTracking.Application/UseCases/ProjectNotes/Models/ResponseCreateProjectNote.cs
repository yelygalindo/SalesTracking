namespace SalesTracking.Application.UseCases.ProjectNotes.Models
{
    public class ResponseCreateProjectNote
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Message { get; set; }
        public CreateProjectNote CreateProjectNote { get; set; }
    }
}
