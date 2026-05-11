namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class CustomerNoteResult
    {
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
