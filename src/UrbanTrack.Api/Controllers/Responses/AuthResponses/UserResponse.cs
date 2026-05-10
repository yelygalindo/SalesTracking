namespace UrbanTrack.Api.Controllers.Responses.AuthResponses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public CompanyResponse Company { get; set; }
        public string Email { get; internal set; }
    }
}