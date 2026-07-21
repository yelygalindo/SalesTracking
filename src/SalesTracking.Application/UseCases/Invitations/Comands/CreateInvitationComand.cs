namespace SalesTracking.Application.UseCases.Invitations.Comands
{
    public record CreateInvitationComand(
        string FullName,
        string Email,
        string RoleCode,
        int CompanyId,
        string InvitedBy);
}
