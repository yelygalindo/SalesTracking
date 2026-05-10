
namespace SalesTracking.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public User User { get; set; }

        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? RevokedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string? ReplacedByTokenHash { get; set; }
    }
}