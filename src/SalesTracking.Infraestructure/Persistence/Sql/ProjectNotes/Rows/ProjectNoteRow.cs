namespace SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes.Rows
{
    internal sealed class ProjectNoteRow
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int? CreatedByUserId { get; set; }
        public string? CreatedByUserExternalId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int? UpdatedByUserId { get; set; }
        public string? UpdatedByUserExternalId { get; set; }
        public string? UpdatedByUserName { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
