using SalesTracking.Application.UseCases.Authentication.Comands;
using SalesTracking.Application.UseCases.Invitations.Comands;
using SalesTracking.Application.UseCases.Invitations.Models;
using UrbanTrack.Api.Controllers.Requests.AuthRequests;
using UrbanTrack.Api.Controllers.Requests.Invitations;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    internal static class AuthRequestMappers
    {
        public static LoginCommand ToApplication(this LoginRequest loginRequest)
        {
            if (loginRequest == null) return null;
            return new LoginCommand()
            {
                Email = loginRequest.Email,
                Password = loginRequest.Password
            };
        }

        public static LogoutComand ToApplication(this LogoutRequest logoutRequest)
        {
            if (logoutRequest == null) return null;
            return new LogoutComand() { RefreshToken = logoutRequest.RefreshToken };
        }

        public static RefreshTokenComand ToApplication(this RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest == null) return null;
            return new RefreshTokenComand() { RefreshToken = refreshTokenRequest.RefreshToken };
        }

        public static ForgotPasswordComand ToApplication(this ForgotPasswordRequest api)
        {
            if (api == null) return null;
            return new ForgotPasswordComand() { Email = api.Email };
        }

        public static ResetPasswordComand ToApplication(this ResetPasswordRequest api)
        {
            if (api == null) return null;
            return new ResetPasswordComand() { Token = api.Token, NewPassword = api.NewPassword };
        }

        public static AcceptInvitationComand ToApplication(this AcceptInvitationInput api)
        {
            if (api == null) return null;
            return new AcceptInvitationComand() { Token = api.Token, Password = api.Password, FullNameUser = api.FullNameUser };
        }

    }
}
