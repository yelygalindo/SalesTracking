using SalesTracking.Application.UseCases.Authentication.Comands;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Models;
using SalesTracking.Application.UseCases.Authentication.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;

        public AuthService(IAuthRepository repo)
        {
            _repo = repo;
        }

        public async Task<LoginResult> LoginAsync(LoginCommand loginCommand)
        {
            var auth = await _repo.ValidateCredentialsAsync(loginCommand.Email, loginCommand.Password);
            if (auth == null)
                return null;

            UserResult userResult = new UserResult
            {
                Id = auth.User.Id,
                FullName = auth.User.FullName,
                ExternalId = auth.User.ExternalId,
                Username = auth.User.Username,
                Email = auth.User.Email,
                Company = new CompanyResult
                {
                    Id = auth.User.Company.Id,
                    ExternalId= auth.User.Company.ExternalId,
                    Name = auth.User.Company.Name
                }
            };

            return new LoginResult
            {
                User = userResult,
                AccessToken = auth.AccessToken,
                RefreshToken = auth.RefreshToken,
                ExpiresAtUtc = auth.ExpiresAtUtc
            };
        }

        public async Task<LogoutResult> LogoutAsync(LogoutComand logoutComand)
        {
            var ok = await _repo.RevokeRefreshTokenAsync(logoutComand.RefreshToken);
            return new LogoutResult { Message = ok ? "Sesión cerrada" : "Token no encontrado" };
        }

        public async Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenComand refreshTokenComand)
        {
            var tokenResult = await _repo.RefreshTokensAsync(refreshTokenComand.RefreshToken);
            if (tokenResult == null)
                return null;

            return new RefreshTokenResult
            {
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                ExpiresAtUtc = tokenResult.ExpiresAtUtc
            };
        }

        public async Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordComand resetPasswordComand)
        {
            var ok = await _repo.ResetPasswordAsync(resetPasswordComand.Token, resetPasswordComand.NewPassword);
            return new ResetPasswordResult { Message = ok ? "Contraseña restablecida" : "Token inválido o expirado" };

        }

        public async Task<InvitationResult?> GetInvitationByTokenAsync(GetInvitationByTokenComand getInvitationByTokenComand)
        {
            Invitation? invitation = await _repo.GetInvitationByTokenAsync(getInvitationByTokenComand.Token);
            if (invitation == null)
                return null;

            return new InvitationResult
            {
                Token = getInvitationByTokenComand.Token,
                Email = invitation.Email,
                InvitedBy = invitation.InvitedBy,
                CompanyId = invitation.CompanyId.ToString(),
                CompanyName = invitation.CompanyName,
                ExpiresAtUtc = invitation.ExpiresAtUtc
            };
        }

        public async Task<AcceptInvitationResult> AcceptInvitationAsync(AcceptInvitationComand request)
        {
            AcceptInvitationInput acceptInvitationInput = new AcceptInvitationInput()
            {
                Token = request.Token,
                Password = request.Password,
                FullNameUser = request.FullNameUser
            };

            AcceptInvitation acceptInvitationResult = await _repo.AcceptInvitationAsync(acceptInvitationInput);
            return new AcceptInvitationResult
            {
                Message = acceptInvitationResult.Succeeded ? "Invitación aceptada" : "Invitación inválida",
                ExternalUserId = acceptInvitationResult.User?.ExternalId
            };
        }

        public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordComand forgotPasswordComand)
        {
            var result = await _repo.SendForgotPasswordAsync(forgotPasswordComand.Email);
            if (result == null)
                return new ForgotPasswordResult { Message = "No se pudo procesar la solicitud" };
            //enviar el correo con el tokenHash para que el usuario pueda resetear la contraseña.
            return new ForgotPasswordResult { Message = $"Instrucciones enviada, token : {result.Token}" };
        }

        public async Task<CreateInvitationResult> CreateInvitationAsync(CreateInvitationComand request)
        {
            CreateInvitation invitation = new()
            {
                Email = request.Email,
                CompanyId = request.CompanyId,
                InvitedBy = request.InvitedBy
            };

            Invitation? created = await _repo.CreateInvitationAsync(invitation);

            return new CreateInvitationResult
            {
                Token = created.TokenHash,
                Email = created.Email,
                ExpiresAtUtc = created.ExpiresAtUtc
            };
        }
    }
}
