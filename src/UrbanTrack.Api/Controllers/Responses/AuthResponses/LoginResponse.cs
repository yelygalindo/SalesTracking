namespace UrbanTrack.Api.Controllers.Responses.AuthResponses
{
    public class LoginResponse
    {
        public UserCompleteResponse User { get; set; }
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}