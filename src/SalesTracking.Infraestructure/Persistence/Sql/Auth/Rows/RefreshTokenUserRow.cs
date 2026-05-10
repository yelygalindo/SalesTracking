namespace SalesTracking.Infrastructure.Persistence.Sql.Auth.Rows
{
    public class RefreshTokenUserRow
    {
        public int RefreshTokenId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyExternalId { get; set; }
        public string CompanyName { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }

    }
}
