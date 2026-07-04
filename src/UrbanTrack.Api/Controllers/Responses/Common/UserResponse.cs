namespace UrbanTrack.Api.Controllers.Responses.Common
{
    public class UserResponse
    {
        public UserResponse(int id, string externalId, string? name)
        {
            Id = id;
            ExternalId = externalId;
            Name = name;
        }

        public UserResponse(int? id, string? externalId, string? name)
        {
            Id = id;
            ExternalId = externalId;
            Name = name;
        }

        public int? Id { get; set; }
        public string? ExternalId { get; set; }
        public string? Name { get; set; }
    }
}