namespace SalesTracking.Domain.Entities
{
    public class CustomerNote
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public int CustomerId { get; set; }
        public string Text { get; set; }
        public Author Author { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
