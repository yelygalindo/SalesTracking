using SalesTracking.Application.Common.Authorization;

namespace SalesTracking.Application.Common.Interfaces;

public interface IUserAuthorizationRepository
{
    Task<UserAuthorizationInfo> GetByUserIdAsync(int userId);
}
