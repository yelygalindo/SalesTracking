using SalesTracking.Application.UseCases.Invitations.Comands;
using SalesTracking.Application.UseCases.Invitations.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Models;
using SalesTracking.Application.UseCases.Invitations.Results;
using SalesTracking.Domain.Entities;
using SalesTracking.Application.Common.Interfaces;

namespace SalesTracking.Application.UseCases.Invitations.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _repo;
        private readonly IPasswordPolicy _passwordPolicy;
        private readonly ICurrentUser _currentUser;

        public InvitationService(IInvitationRepository repo, IPasswordPolicy passwordPolicy, ICurrentUser currentUser)
        {
            _repo = repo;
            _passwordPolicy = passwordPolicy;
            _currentUser = currentUser;
        }
     
        public async Task<InvitationResult?> GetInvitationByTokenAsync(GetInvitationByTokenComand getInvitationByTokenComand)
        {
            Invitation? invitation = await _repo.GetInvitationByTokenAsync(getInvitationByTokenComand.Token);
            if (invitation == null)
                return null;

            return new InvitationResult
            {
                Email = invitation.Email,
                FullName = invitation.FullName,
                RoleCode = invitation.RoleCode,
                InvitedBy = invitation.InvitedBy,
                CompanyId = invitation.CompanyId.ToString(),
                CompanyName = invitation.CompanyName,
                ExpiresAtUtc = invitation.ExpiresAtUtc
            };
        }

        public async Task<AcceptInvitationResult> AcceptInvitationAsync(AcceptInvitationComand request)
        {
            if (request == null)
                return new AcceptInvitationResult { Message = "Invitación inválida" };
            var validation = _passwordPolicy.Validate(request.Password);
            if (!validation.IsValid)
                return new AcceptInvitationResult { Message = validation.Error! };

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
            if (request == null || string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.RoleCode))
                return new CreateInvitationResult { Message = "Los datos de la invitación son requeridos." };

            CreateInvitation invitation = new()
            {
                Email = request.Email.Trim(),
                FullName = request.FullName.Trim(),
                RoleCode = request.RoleCode.Trim(),
                CompanyId = _currentUser.CompanyId,
                InvitedBy = _currentUser.UserExternalId,
                InviterPermissions = _currentUser.Permissions
            };

            Invitation? created = await _repo.CreateInvitationAsync(invitation);

            return new CreateInvitationResult
            {
                Succeeded = created != null,
                Email = created?.Email ?? request.Email,
                ExpiresAtUtc = created?.ExpiresAtUtc ?? default,
                Message = created != null ? "Invitación creada correctamente." : "No se pudo crear la invitación."
            };
        }
    }
}
