using BCrypt.Net;
using SalesTracking.Application.UseCases.Authentication.Interfaces;

namespace SalesTracking.Infrastructure.Persistence.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {           
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
