namespace UrbanTrack.Api.Controllers.Requests.AuthRequests
{
    public class LoginRequest
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string DeviceType { get; init; } = "Web";
        public string? DeviceId { get; init; }
        public string? PushToken { get; init; }
    }
}