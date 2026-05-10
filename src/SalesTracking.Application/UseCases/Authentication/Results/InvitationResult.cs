namespace SalesTracking.Application.UseCases.Authentication.Results
{
    public class InvitationResult
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string InvitedBy { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}
