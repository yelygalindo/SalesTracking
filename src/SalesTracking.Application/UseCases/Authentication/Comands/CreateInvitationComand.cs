namespace SalesTracking.Application.UseCases.Authentication.Comands
{
    public record CreateInvitationComand(
        string Email,
        int CompanyId,
        string InvitedBy);
}
