namespace SalesTracking.Infrastructure.Persistence.Sql.Customers.Rows
{
    public class CustomerNoteRow
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
