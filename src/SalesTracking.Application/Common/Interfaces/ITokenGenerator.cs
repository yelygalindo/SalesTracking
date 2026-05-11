using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(User user, DateTime expiresAtUtc);
        string GeneratePasswordResetToken();
        string GenerateRefreshToken();
    }
}