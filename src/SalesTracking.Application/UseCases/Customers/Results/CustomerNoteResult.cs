namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class CustomerNoteResult
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public AuthorNoteResult Author { get; set; }        
        public DateTime CreatedAtUtc { get; set; }
    }
}
