namespace SalesTracking.Application.UseCases.ProjectAttachments.Results
{
    public sealed class SetProjectAttachmentCoverResult
    {
        public bool Succeeded { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
