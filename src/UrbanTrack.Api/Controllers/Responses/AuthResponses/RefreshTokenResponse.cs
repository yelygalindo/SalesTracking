namespace UrbanTrack.Api.Controllers.Responses.AuthResponses
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}
