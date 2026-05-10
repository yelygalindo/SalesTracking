namespace UrbanTrack.Api.Controllers.Responses.AuthResponses
{
    public class CreateInvitationResponse
    {
        public string Token { get; init; }
        public string Email { get; init; }
        public DateTime ExpiresAtUtc { get; init; }
    }
}