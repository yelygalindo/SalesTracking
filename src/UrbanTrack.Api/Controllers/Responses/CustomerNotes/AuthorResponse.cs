namespace UrbanTrack.Api.Controllers.Responses.CustomerNotes
{
    public class AuthorResponse
    {
        public AuthorResponse(int id, string externalId, string? name)
        {
            Id = id;
            ExternalId = externalId;
            Name = name;
        }

        public int Id { get; }
        public string ExternalId { get; }
        public string Name { get; }
    }
}
