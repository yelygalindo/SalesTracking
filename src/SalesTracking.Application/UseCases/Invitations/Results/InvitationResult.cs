namespace SalesTracking.Application.UseCases.Invitations.Results
{
    public class InvitationResult
    {
        public string Email { get; set; }
        public string FullName { get; set; } = default!;
        public string RoleCode { get; set; } = default!;
        public string InvitedBy { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}
