namespace UrbanTrack.Api.Controllers.Requests.AuthRequests
{
    public class AuthUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public CompanyDto Company { get; set; } = new();
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
