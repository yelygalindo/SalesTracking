namespace SalesTracking.Infrastructure.Persistence.Sql.Auth.Rows
{
    public class PasswordResetTokenRow
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? UsedAtUtc { get; set; }
    }
}
