using SalesTracking.Application.UseCases.Invitations.Comands;
using SalesTracking.Application.UseCases.Invitations.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Models;
using SalesTracking.Application.UseCases.Invitations.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Invitations.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _repo;

        public InvitationService(IInvitationRepository repo)
        {
            _repo = repo;
        }
     
        public async Task<InvitationResult?> GetInvitationByTokenAsync(GetInvitationByTokenComand getInvitationByTokenComand)
        {
            Invitation? invitation = await _repo.GetInvitationByTokenAsync(getInvitationByTokenComand.Token);
            if (invitation == null)
                return null;

            return new InvitationResult
            {
                Token = getInvitationByTokenComand.Token,
                Email = invitation.Email,
                InvitedBy = invitation.InvitedBy,
                CompanyId = invitation.CompanyId.ToString(),
                CompanyName = invitation.CompanyName,
                ExpiresAtUtc = invitation.ExpiresAtUtc
            };
        }

        public async Task<AcceptInvitationResult> AcceptInvitationAsync(AcceptInvitationComand request)
        {
            AcceptInvitationInput acceptInvitationInput = new AcceptInvitationInput()
            {
                Token = request.Token,
                Password = request.Password,
                FullNameUser = request.FullNameUser
            };

            AcceptInvitation acceptInvitationResult = await _repo.AcceptInvitationAsync(acceptInvitationInput);
            return new AcceptInvitationResult
            {
                Message = acceptInvitationResult.Succeeded ? "Invitación aceptada" : "Invitación inválida",
                ExternalUserId = acceptInvitationResult.User?.ExternalId
            };
        }

        public async Task<CreateInvitationResult> CreateInvitationAsync(CreateInvitationComand request)
        {
            //TODO: deberia agregar el rol al cual se está enviando la invitación , para que el usuario tenga los permisos adecuados al aceptar la invitación.
            CreateInvitation invitation = new()
            {
                Email = request.Email,
                CompanyId = request.CompanyId,
                InvitedBy = request.InvitedBy
            };

            Invitation? created = await _repo.CreateInvitationAsync(invitation);

            return new CreateInvitationResult
            {
                Token = created.TokenHash,
                Email = created.Email,
                ExpiresAtUtc = created.ExpiresAtUtc
            };
        }
    }
}
