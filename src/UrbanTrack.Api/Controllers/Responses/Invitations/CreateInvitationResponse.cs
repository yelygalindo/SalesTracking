namespace UrbanTrack.Api.Controllers.Responses.Invitations
{
    public class CreateInvitationResponse
    {
        public string Email { get; init; }
        public string Message { get; init; } = default!;
        public DateTime ExpiresAtUtc { get; init; }
    }
}
