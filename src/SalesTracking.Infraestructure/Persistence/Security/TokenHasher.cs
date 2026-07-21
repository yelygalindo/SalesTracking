using System.Security.Cryptography;
using System.Text;

namespace SalesTracking.Infrastructure.Persistence.Security;

internal static class TokenHasher
{
    public static string Hash(string token)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
