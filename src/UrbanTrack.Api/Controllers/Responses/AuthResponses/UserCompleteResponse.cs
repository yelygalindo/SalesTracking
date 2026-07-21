namespace UrbanTrack.Api.Controllers.Responses.AuthResponses
{
    public class UserCompleteResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public CompanyResponse Company { get; set; }
        public string Email { get; internal set; }
        public IReadOnlyCollection<string> Roles { get; set; } = [];
        public IReadOnlyCollection<string> Permissions { get; set; } = [];
    }
}
