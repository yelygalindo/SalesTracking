using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Models;
using SalesTracking.Infrastructure.Persistence.Settings;

namespace SalesTracking.Host.Development;

public sealed class DevelopmentInvitationLinkNotifier : IInvitationLinkNotifier
{
    private readonly ILogger<DevelopmentInvitationLinkNotifier> _logger;
    private readonly FrontendLinkSettings _linkSettings;

    public DevelopmentInvitationLinkNotifier(
        ILogger<DevelopmentInvitationLinkNotifier> logger,
        IOptions<FrontendLinkSettings> linkSettings)
    {
        _logger = logger;
        _linkSettings = linkSettings.Value;
    }

    public Task NotifyAsync(CreatedInvitation invitation)
    {
        string separator = _linkSettings.InvitationUrl.Contains('?') ? "&" : "?";
        string link =
            $"{_linkSettings.InvitationUrl}{separator}token={Uri.EscapeDataString(invitation.Token)}";

        _logger.LogInformation(
            "Development invitation link for {Email}, expires at {ExpiresAtUtc}: {InvitationLink}",
            invitation.Email,
            invitation.ExpiresAtUtc,
            link);

        return Task.CompletedTask;
    }
}
