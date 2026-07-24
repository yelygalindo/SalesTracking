namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments.Rows
{
    internal sealed class ProjectAttachmentRow
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string AttachmentType { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
        public string UploadedByUserExternalId { get; set; } = string.Empty;
        public string UploadedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
