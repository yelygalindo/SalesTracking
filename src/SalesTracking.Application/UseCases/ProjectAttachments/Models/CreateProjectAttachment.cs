namespace SalesTracking.Application.UseCases.ProjectAttachments.Models
{
    public sealed class CreateProjectAttachment
    {
        public string ExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string StorageProvider { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string AttachmentType { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
        public string UploadedByUserExternalId { get; set; } = string.Empty;
    }
}
