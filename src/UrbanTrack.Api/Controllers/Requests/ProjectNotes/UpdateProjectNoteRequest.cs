namespace UrbanTrack.Api.Controllers.Requests.ProjectNotes
{
    public class UpdateProjectNoteRequest
    {
        public string Content { get; set; }
        public string UpdatedByUserExternalId { get; set; }
    }
}