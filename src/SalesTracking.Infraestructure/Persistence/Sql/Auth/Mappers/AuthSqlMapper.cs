using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Sql.Auth.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.Auth.Mappers
{
    public static class AuthSqlMapper
    {
        public static User ToAuthUserResponse(this RefreshTokenUserRow e)
        {
            if (e == null) return null;
            return new User
            {
                Id = e.UserId,
                Email = e.Email,
                Username = e.Username,
                FullName = e.FullName,
                Company = new Company(e.CompanyId, e.CompanyExternalId, e.CompanyName)
            };
        }

        public static User ToAuthUserResponse(this AuthUserRow e)
        {
            if (e == null) return null;
            return new User
            {
                Id = e.Id,
                Username = e.Username,
                FullName = e.FullName,
                ExternalId = e.ExternalId,
                Email = e.Email,
                Company = new Company(e.CompanyId,e.CompanyExternalId, e.CompanyName)
            };
        }
    }
}
