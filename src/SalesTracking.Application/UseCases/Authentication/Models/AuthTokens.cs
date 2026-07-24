using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Authentication.Models
{
    public class AuthTokens
    {

        public User User { get; set; }
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public IReadOnlyCollection<string> Roles { get; set; } = [];

        public IReadOnlyCollection<string> Permissions { get; set; } = [];
    }
}
