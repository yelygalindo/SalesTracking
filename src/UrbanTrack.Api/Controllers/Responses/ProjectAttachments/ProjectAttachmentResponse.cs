namespace UrbanTrack.Api.Controllers.Responses.ProjectAttachments
{
    public sealed class ProjectAttachmentResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
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
