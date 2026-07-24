using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Models;
using SalesTracking.Infrastructure.Persistence.Settings;

namespace SalesTracking.Host.Development
{
    public sealed class DevelopmentPasswordResetLinkNotifier : IPasswordResetLinkNotifier
    {
        private readonly ILogger<DevelopmentPasswordResetLinkNotifier> _logger;
        private readonly AuthSettings _authSettings;

        public DevelopmentPasswordResetLinkNotifier(
            ILogger<DevelopmentPasswordResetLinkNotifier> logger,
            IOptions<AuthSettings> authSettings)
        {
            _logger = logger;
            _authSettings = authSettings.Value;
        }

        public Task NotifyAsync(PasswordForgot passwordForgot)
        {
            string separator = _authSettings.PasswordResetUrl.Contains('?') ? "&" : "?";
            string link = $"{_authSettings.PasswordResetUrl}{separator}token={Uri.EscapeDataString(passwordForgot.Token)}";

            _logger.LogInformation(
                "Development password reset link for {Email}: {PasswordResetLink}",
                passwordForgot.Email,
                link);

            return Task.CompletedTask;
        }
    }
}
