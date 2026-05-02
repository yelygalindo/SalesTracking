namespace UrbanTrack.Api.Controllers.Requests.AuthRequests
{
    public class LogoutRequest
    {
        public string RefreshToken { get; init; } = string.Empty;
        public string? DeviceId { get; init; }
    }
}
