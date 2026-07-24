using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Models;
using SalesTracking.Infrastructure.Persistence.Settings;

namespace SalesTracking.Host.Development;

public sealed class DevelopmentInvitationLinkNotifier : IInvitationLinkNotifier
{
    private readonly ILogger<DevelopmentInvitationLinkNotifier> _logger;
    private readonly AuthSettings _authSettings;

    public DevelopmentInvitationLinkNotifier(
        ILogger<DevelopmentInvitationLinkNotifier> logger,
        IOptions<AuthSettings> authSettings)
    {
        _logger = logger;
        _authSettings = authSettings.Value;
    }

    public Task NotifyAsync(CreatedInvitation invitation)
    {
        string separator = _authSettings.InvitationUrl.Contains('?') ? "&" : "?";
        string link =
            $"{_authSettings.InvitationUrl}{separator}token={Uri.EscapeDataString(invitation.Token)}";

        _logger.LogInformation(
            "Development invitation link for {Email}, expires at {ExpiresAtUtc}: {InvitationLink}",
            invitation.Email,
            invitation.ExpiresAtUtc,
            link);

        return Task.CompletedTask;
    }
}
