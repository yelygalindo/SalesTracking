namespace SalesTracking.Application.UseCases.ProjectAttachments.Results
{
    public sealed class ProjectAttachmentContentResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public Stream? Content { get; set; }
    }
}
