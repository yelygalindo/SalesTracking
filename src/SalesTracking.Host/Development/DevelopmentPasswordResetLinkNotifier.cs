using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Models;
using SalesTracking.Infrastructure.Persistence.Settings;

namespace SalesTracking.Host.Development
{
    public sealed class DevelopmentPasswordResetLinkNotifier : IPasswordResetLinkNotifier
    {
        private readonly ILogger<DevelopmentPasswordResetLinkNotifier> _logger;
        private readonly FrontendLinkSettings _linkSettings;

        public DevelopmentPasswordResetLinkNotifier(
            ILogger<DevelopmentPasswordResetLinkNotifier> logger,
            IOptions<FrontendLinkSettings> linkSettings)
        {
            _logger = logger;
            _linkSettings = linkSettings.Value;
        }

        public Task NotifyAsync(PasswordForgot passwordForgot)
        {
            string separator = _linkSettings.PasswordResetUrl.Contains('?') ? "&" : "?";
            string link = $"{_linkSettings.PasswordResetUrl}{separator}token={Uri.EscapeDataString(passwordForgot.Token)}";

            _logger.LogInformation(
                "Development password reset link for {Email}: {PasswordResetLink}",
                passwordForgot.Email,
                link);

            return Task.CompletedTask;
        }
    }
}
