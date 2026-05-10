using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Authentication.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(User user, DateTime expiresAtUtc);
        string GeneratePasswordResetToken();
        string GenerateRefreshToken();
    }
}