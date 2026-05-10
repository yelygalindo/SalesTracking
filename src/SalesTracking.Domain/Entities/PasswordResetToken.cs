namespace SalesTracking.Domain.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? UsedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
