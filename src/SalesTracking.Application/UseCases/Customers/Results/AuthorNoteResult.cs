namespace SalesTracking.Application.UseCases.Customers.Results
{
    public class AuthorNoteResult
    {       
        public AuthorNoteResult(int id, string? externalId, string? name)
        {
            Id = id;
            Name = name;
            ExternalId = externalId;
        }

        public int Id { get; set; }
        public string? ExternalId { get; set; }
        public string? Name { get; set; }     
    }
}
