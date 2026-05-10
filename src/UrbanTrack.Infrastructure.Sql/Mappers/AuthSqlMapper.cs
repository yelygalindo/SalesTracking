using UrbanTrack.Infrastructure.Sql.Entities;
using UrbanTrack.Application.DTOs.Responses.Auth;

namespace UrbanTrack.Infrastructure.Sql.Mappers
{
    public static class AuthSqlMapper
    {
        public static AuthUserResponse ToAuthUserResponse(this AuthUserSqlEntity e)
        {
            if (e == null) return null;
            return new AuthUserResponse
            {
                Id = e.Id,
                Username = e.Username,
                FullName = e.FullName,
                Company = e.Company == null ? null : new CompanyResponse { Id = e.Company.Id, Name = e.Company.Name }
            };
        }

        public static InvitationResponse ToInvitationResponse(this InvitationSqlEntity e)
        {
            if (e == null) return null;
            return new InvitationResponse
            {
                Token = e.Token,
                Email = e.Email,
                InvitedBy = e.InvitedBy,
                CompanyId = e.CompanyId,
                CompanyName = e.CompanyName,
                ExpiresAtUtc = e.ExpiresAtUtc
            };
        }
    }
}