namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments.Rows
{
    internal sealed class ProjectAttachmentContentRow
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
    }
}
