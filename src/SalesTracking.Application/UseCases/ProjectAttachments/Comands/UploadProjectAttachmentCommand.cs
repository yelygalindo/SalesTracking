namespace SalesTracking.Application.UseCases.ProjectAttachments.Comands
{
    public sealed class UploadProjectAttachmentCommand
    {
        public string ProjectExternalId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public Stream Content { get; set; } = Stream.Null;
        public string AttachmentType { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
        public int UploadedByUserId { get; set; }
    }
}