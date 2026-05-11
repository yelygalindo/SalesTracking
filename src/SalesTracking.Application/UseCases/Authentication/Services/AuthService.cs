using SalesTracking.Application.UseCases.Authentication.Comands;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Results;

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

        public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordComand forgotPasswordComand)
        {
            var result = await _repo.SendForgotPasswordAsync(forgotPasswordComand.Email);
            if (result == null)
                return new ForgotPasswordResult { Message = "No se pudo procesar la solicitud" };
            //enviar el correo con el tokenHash para que el usuario pueda resetear la contraseña.
            return new ForgotPasswordResult { Message = $"Instrucciones enviada, token : {result.Token}" };
        }
    }
}
