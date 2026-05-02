namespace UrbanTrack.Api.Controllers.Requests.AuthRequests
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public AuthUserDto User { get; set; } = new();
    }
}