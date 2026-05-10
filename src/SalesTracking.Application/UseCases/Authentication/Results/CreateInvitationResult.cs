
namespace SalesTracking.Application.UseCases.Authentication.Results
{
    public class CreateInvitationResult
    {
        public string Token { get; init; }
        public string Email { get; init; }
        public DateTime ExpiresAtUtc { get; init; }
    }
}
