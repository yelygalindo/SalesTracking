namespace SalesTracking.Application.UseCases.Invitations.Models
{
    public class CreateInvitation
    {
        public string Email { get; init; }
        public string FullName { get; init; } = default!;
        public string RoleCode { get; init; } = default!;
        public IReadOnlyCollection<string> InviterPermissions { get; init; } = [];
        public int CompanyId { get; init; }
        public string InvitedBy { get; init; }
    }
}
