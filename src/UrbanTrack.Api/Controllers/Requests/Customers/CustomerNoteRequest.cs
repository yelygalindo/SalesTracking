namespace UrbanTrack.Api.Controllers.Requests.Customers
{
    public class CustomerNoteRequest
    {
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}