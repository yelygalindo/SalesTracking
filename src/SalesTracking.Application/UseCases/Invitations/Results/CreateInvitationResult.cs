namespace SalesTracking.Application.UseCases.Invitations.Results
{
    public class CreateInvitationResult
    {
        public bool Succeeded { get; init; }
        public string Email { get; init; }
        public string Message { get; init; } = default!;
        public DateTime ExpiresAtUtc { get; init; }
    }
}
