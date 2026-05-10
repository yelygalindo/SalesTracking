
namespace SalesTracking.Application.UseCases.Authentication.Models
{
    public class PasswordForgot
    {
        public string Email { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresAtUtc { get; set; }
    }
}
