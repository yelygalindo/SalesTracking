namespace SalesTracking.Domain.Entities
{
    public class Author
    {       
        public Author(int authorId, string authorExternalId, string authorName)
        {
            Id = authorId;
            ExternalId = authorExternalId;
            Name = authorName;
        }

        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string? Name { get; set; }
    }
}
