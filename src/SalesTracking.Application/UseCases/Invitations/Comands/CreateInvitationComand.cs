namespace SalesTracking.Application.UseCases.Invitations.Comands
{
    public record CreateInvitationComand(
        string Email,
        int CompanyId,
        string InvitedBy);
}
