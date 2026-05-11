namespace UrbanTrack.Api.Controllers.Responses.Invitations
{
    public class CreateInvitationResponse
    {
        public string Token { get; init; }
        public string Email { get; init; }
        public DateTime ExpiresAtUtc { get; init; }
    }
}