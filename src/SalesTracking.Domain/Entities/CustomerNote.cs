namespace SalesTracking.Domain.Entities
{
    public class CustomerNote
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }

        public int CustomerId { get; set; }
        public string Text { get; set; }

        public int AuthorId { get; set; }
        public string ExternalAuthorId { get; set; }
        public string? AuthorName { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
