namespace SalesTracking.Application.UseCases.ProjectNotes.Models
{
    public class ResponseUpdateProjectNote
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Message { get; set; }
    }
}