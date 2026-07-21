namespace SalesTracking.Domain.Entities
{
    public class Invitation
    {
        public int Id { get; set; }

        public string TokenHash { get; set; } = default!;

        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string RoleCode { get; set; } = default!;

        public string InvitedBy { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? AcceptedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
