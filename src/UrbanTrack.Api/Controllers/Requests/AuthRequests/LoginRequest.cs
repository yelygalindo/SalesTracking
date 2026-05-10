namespace UrbanTrack.Api.Controllers.Requests.AuthRequests
{
    public class LoginRequest
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string DeviceType { get; init; } = "Web";
        public string? DeviceId { get; init; }
    }
}