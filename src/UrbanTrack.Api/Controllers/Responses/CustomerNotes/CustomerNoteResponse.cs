namespace UrbanTrack.Api.Controllers.Responses.CustomerNotes
{
    public class CustomerNoteResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public AuthorResponse Author { get; set; }        
        public DateTime CreatedAt { get; set; }
    }
}