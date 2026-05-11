using SalesTracking.Application.UseCases.Invitations.Comands;
using SalesTracking.Application.UseCases.Invitations.Results;

namespace SalesTracking.Application.UseCases.Invitations.Interfaces
{
    public interface IInvitationService
    {     
        Task<InvitationResult?> GetInvitationByTokenAsync(GetInvitationByTokenComand getInvitationByTokenComand);
        Task<AcceptInvitationResult> AcceptInvitationAsync(AcceptInvitationComand acceptInvitationComand);
        Task<CreateInvitationResult> CreateInvitationAsync(CreateInvitationComand createInvitationComand);
    }
}