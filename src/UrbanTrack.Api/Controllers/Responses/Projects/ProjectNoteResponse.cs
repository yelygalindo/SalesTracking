namespace UrbanTrack.Api.Controllers.Responses.Projects
{
    public sealed class ProjectNoteResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ProjectNoteUserResponse? CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public ProjectNoteUserResponse? UpdatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
