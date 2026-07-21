namespace SalesTracking.Infrastructure.Persistence.Sql.Invitations.Rows
{
    public class InvitationRow
    {
        public string TokenHash { get; set; }

        public string Email { get; set; }
        public string FullName { get; set; } = default!;
        public string RoleCode { get; set; } = default!;

        public string InvitedBy { get; set; }

        public int CompanyId { get; set; }
        public string CompanyExternalId { get; set; }
        public string CompanyName { get; set; }

        public DateTime ExpiresAtUtc { get; set; }
    }
}
