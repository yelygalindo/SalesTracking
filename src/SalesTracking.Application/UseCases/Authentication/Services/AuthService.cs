using SalesTracking.Application.UseCases.Authentication.Comands;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Results;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.Common.Authentication;

namespace SalesTracking.Application.UseCases.Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IPasswordPolicy _passwordPolicy;
        private readonly IUserAuthorizationRepository? _authorizationRepository;
        private readonly IPasswordResetLinkNotifier? _passwordResetLinkNotifier;

        public AuthService(
            IAuthRepository repo,
            IPasswordPolicy? passwordPolicy = null,
            IUserAuthorizationRepository? authorizationRepository = null,
            IPasswordResetLinkNotifier? passwordResetLinkNotifier = null)
        {
            _repo = repo;
            _passwordPolicy = passwordPolicy ?? new SalesTracking.Application.Common.Validation.PasswordPolicy();
            _authorizationRepository = authorizationRepository;
            _passwordResetLinkNotifier = passwordResetLinkNotifier;
        }

        public async Task<LoginResult?> LoginAsync(LoginCommand loginCommand)
        {
            if (loginCommand == null || string.IsNullOrWhiteSpace(loginCommand.Email) || string.IsNullOrWhiteSpace(loginCommand.Password))
                return null;
            if (!DeviceTypes.IsSupported(loginCommand.DeviceType))
                return null;
            if (loginCommand.DeviceId?.Trim().Length > 100)
                return null;

            var auth = await _repo.ValidateCredentialsAsync(
                loginCommand.Email.Trim(),
                loginCommand.Password,
                loginCommand.DeviceId?.Trim());
            if (auth == null)
                return null;

            UserResult userResult = new UserResult
            {
                Id = auth.User.Id,
                FullName = auth.User.FullName,
                ExternalId = auth.User.ExternalId,
                Username = auth.User.Username,
                Email = auth.User.Email,
                Roles = auth.Roles,
                Permissions = auth.Permissions,
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

            var ok = await _repo.RevokeRefreshTokenAsync(
                logoutComand.RefreshToken.Trim(),
                logoutComand.DeviceId?.Trim());
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

            var validation = _passwordPolicy.Validate(resetPasswordComand.NewPassword);
            if (!validation.IsValid)
                return new ResetPasswordResult { Succeeded = false, Message = validation.Error! };

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

            var passwordForgot = await _repo.SendForgotPasswordAsync(forgotPasswordComand.Email.Trim());
            if (passwordForgot != null && _passwordResetLinkNotifier != null)
                await _passwordResetLinkNotifier.NotifyAsync(passwordForgot);

            return new ForgotPasswordResult { Message = message };
        }

        public async Task<AuthMeResult?> GetMeAsync(int userId)
        {
            var user = await _repo.GetUserByIdAsync(userId);
            if (user == null) return null;
            if (_authorizationRepository == null) return null;
            var authorization = await _authorizationRepository.GetByUserIdAsync(userId);
            return new AuthMeResult
            {
                Id = user.Id,
                ExternalId = user.ExternalId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                CompanyId = user.Company.Id,
                CompanyExternalId = user.Company.ExternalId,
                CompanyName = user.Company.Name,
                Roles = authorization.Roles,
                Permissions = authorization.Permissions
            };
        }
    }
}
