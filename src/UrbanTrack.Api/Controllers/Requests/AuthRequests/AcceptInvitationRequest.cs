namespace UrbanTrack.Api.Controllers.Requests.AuthRequests
{
    public class AcceptInvitationRequest
    {
        public string InvitationToken { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
