namespace SalesTracking.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string ExternalId { get; set; } = default!;

        public string Username { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string FullName { get; set; } = default!;

        public string PasswordHash { get; set; } = default!;

        public Company Company { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
