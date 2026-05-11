namespace UrbanTrack.Api.Controllers.Responses.Customers
{
    public class CustomerNoteResponse
    {
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}