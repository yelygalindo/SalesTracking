namespace SalesTracking.Infrastructure.Persistence.Sql.Auth.Rows
{
    public class AuthUserRow
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public string CompanyExternalId { get; set; }
        public string CompanyName { get; set; }
    }
}
