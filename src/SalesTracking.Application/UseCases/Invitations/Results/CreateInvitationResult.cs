namespace SalesTracking.Application.UseCases.Invitations.Results
{
    public class CreateInvitationResult
    {
        public string Token { get; init; }
        public string Email { get; init; }
        public DateTime ExpiresAtUtc { get; init; }
    }
}
