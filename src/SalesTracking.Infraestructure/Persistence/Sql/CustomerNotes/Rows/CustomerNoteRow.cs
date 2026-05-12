namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes.Rows
{
    public class CustomerNoteRow
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public string AuthorExternalId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}