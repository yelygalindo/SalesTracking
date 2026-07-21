namespace SalesTracking.Application.UseCases.ProjectAttachments.Results
{
    public sealed class GetProjectAttachmentsResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
        public IReadOnlyList<ProjectAttachmentResult> Items { get; set; } = new List<ProjectAttachmentResult>();
    }
}
