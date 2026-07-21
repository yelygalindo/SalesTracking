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

        public async Task<LoginResult?> LoginAsync(LoginCommand loginCommand)
        {
            if (loginCommand == null || string.IsNullOrWhiteSpace(loginCommand.Email) || string.IsNullOrWhiteSpace(loginCommand.Password))
                return null;

            var auth = await _repo.ValidateCredentialsAsync(loginCommand.Email.Trim(), loginCommand.Password);
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
            if (logoutComand == null || string.IsNullOrWhiteSpace(logoutComand.RefreshToken))
                return new LogoutResult { Message = "El refresh token es requerido." };

            var ok = await _repo.RevokeRefreshTokenAsync(logoutComand.RefreshToken.Trim());
            return new LogoutResult { Message = ok ? "Sesión cerrada" : "Token no encontrado" };
        }

        public async Task<RefreshTokenResult?> RefreshTokenAsync(RefreshTokenComand refreshTokenComand)
        {
            if (refreshTokenComand == null || string.IsNullOrWhiteSpace(refreshTokenComand.RefreshToken))
                return null;

            var tokenResult = await _repo.RefreshTokensAsync(refreshTokenComand.RefreshToken.Trim());
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
            if (resetPasswordComand == null || string.IsNullOrWhiteSpace(resetPasswordComand.Token))
                return new ResetPasswordResult { Succeeded = false, Message = "Token inválido o expirado" };

            if (string.IsNullOrWhiteSpace(resetPasswordComand.NewPassword) || resetPasswordComand.NewPassword.Length < 8)
                return new ResetPasswordResult { Succeeded = false, Message = "La contraseña debe tener al menos 8 caracteres" };

            var ok = await _repo.ResetPasswordAsync(resetPasswordComand.Token.Trim(), resetPasswordComand.NewPassword);
            return new ResetPasswordResult
            {
                Succeeded = ok,
                Message = ok ? "Contraseña restablecida" : "Token inválido o expirado"
            };
        }

        public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordComand forgotPasswordComand)
        {
            const string message = "Si el correo está registrado, recibirás instrucciones para restablecer tu contraseña.";

            if (forgotPasswordComand == null || string.IsNullOrWhiteSpace(forgotPasswordComand.Email))
                return new ForgotPasswordResult { Message = message };

            await _repo.SendForgotPasswordAsync(forgotPasswordComand.Email.Trim());
            return new ForgotPasswordResult { Message = message };
        }
    }
}
