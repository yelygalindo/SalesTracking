namespace SalesTracking.Application.UseCases.ProjectAttachments.Results
{
    public sealed class UploadProjectAttachmentResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string? Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
