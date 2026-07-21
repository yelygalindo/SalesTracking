using SalesTracking.Application.Common.Authorization;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(User user, UserAuthorizationInfo authorization, DateTime expiresAtUtc);
        string GeneratePasswordResetToken();
        string GenerateRefreshToken();
    }
}
