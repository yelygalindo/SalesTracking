using SalesTracking.Application.UseCases.Invitations.Models;

namespace SalesTracking.Application.Common.Interfaces;

public interface IInvitationLinkNotifier
{
    Task NotifyAsync(CreatedInvitation invitation);
}
