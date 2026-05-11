using SalesTracking.Application.UseCases.Authentication.Results;
using SalesTracking.Application.UseCases.Invitations.Results;
using UrbanTrack.Api.Controllers.Responses.AuthResponses;
using UrbanTrack.Api.Controllers.Responses.Invitations;

namespace UrbanTrack.Api.Controllers.Responses.Mappers
{
    internal static class AuthResponsesMappers
    {
        public static LoginResponse ToResponse(this LoginResult loginResult)
        {
            if (loginResult == null) return null;
            return new LoginResponse()
            {
                User = loginResult.User.ToResponse(),
                AccessToken = loginResult.AccessToken,
                RefreshToken = loginResult.RefreshToken,
                ExpiresAtUtc = loginResult.ExpiresAtUtc,
            };
        }

        public static UserResponse ToResponse(this UserResult userResult)
        {
            if (userResult == null) return null;
            return new UserResponse()
            {
                Id = userResult.Id,
                ExternalId = userResult.ExternalId,
                Username = userResult.Username,
                FullName = userResult.FullName,
                Company = userResult.Company.ToResponse(),
                Email = userResult.Email
            };
        }

        public static CompanyResponse ToResponse(this CompanyResult companyResult)
        {
            if (companyResult == null) return null;
            return new CompanyResponse()
            {
                Id = companyResult.Id,
                ExternalId = companyResult.ExternalId,
                Name = companyResult.Name
            };
        }

        public static LogoutResponse ToResponse(this LogoutResult logoutRequest)
        {
            if (logoutRequest == null) return null;
            return new LogoutResponse() { Message = logoutRequest.Message };
        }

        public static ForgotPasswordResponse ToResponse(this ForgotPasswordResult logoutRequest)
        {
            if (logoutRequest == null) return null;
            return new ForgotPasswordResponse() { Message = logoutRequest.Message };
        }
        public static ResetPasswordResponse ToResponse(this ResetPasswordResult logoutRequest)
        {
            if (logoutRequest == null) return null;
            return new ResetPasswordResponse() { Message = logoutRequest.Message };
        }

        public static AcceptInvitationResponse ToResponse(this AcceptInvitationResult logoutRequest)
        {
            if (logoutRequest == null) return null;
            return new AcceptInvitationResponse() { Message = logoutRequest.Message };
        }

        public static RefreshTokenResponse ToResponse(this RefreshTokenResult refreshTokenRequest)
        {
            if (refreshTokenRequest == null) return null;
            return new RefreshTokenResponse()
            {
                RefreshToken = refreshTokenRequest.RefreshToken,
                AccessToken = refreshTokenRequest.AccessToken,
                ExpiresAtUtc = refreshTokenRequest.ExpiresAtUtc
            };

        }
        public static InvitationResponse ToResponse(this InvitationResult api)
        {
            if (api == null) return null;
            return new InvitationResponse()
            {
                Token = api.Token,
                Email = api.Email,
                CompanyId = api.CompanyId,
                CompanyName = api.CompanyName,
                ExpiresAtUtc = api.ExpiresAtUtc,
                InvitedBy = api.InvitedBy
            };
        }

        public static CreateInvitationResponse ToResponse(this CreateInvitationResult api)
        {
            if (api == null) return null;
            return new CreateInvitationResponse()
            {
                Token = api.Token,
                Email = api.Email,
                ExpiresAtUtc = api.ExpiresAtUtc,
            };
        }
        
    }
}
