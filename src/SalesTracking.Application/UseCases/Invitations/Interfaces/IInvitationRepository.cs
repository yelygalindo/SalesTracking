using SalesTracking.Application.UseCases.Invitations.Models;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Invitations.Interfaces
{
    public interface IInvitationRepository
    {        
        Task<Invitation?> GetInvitationByTokenAsync(string token);
        Task<AcceptInvitation> AcceptInvitationAsync(AcceptInvitationInput request);        
        Task<CreatedInvitation?> CreateInvitationAsync(CreateInvitation createInvitation);
    }
}
