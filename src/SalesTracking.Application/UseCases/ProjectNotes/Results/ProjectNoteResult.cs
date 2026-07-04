namespace SalesTracking.Application.UseCases.ProjectNotes.Results
{
    public sealed class ProjectNoteResult
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ProjectNoteUserResult? CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public ProjectNoteUserResult? UpdatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
