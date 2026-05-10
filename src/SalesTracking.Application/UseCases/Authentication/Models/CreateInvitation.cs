namespace SalesTracking.Application.UseCases.Authentication.Models
{
    public class CreateInvitation
    {
        public string Email { get; init; }
        public int CompanyId { get; init; }
        public string InvitedBy { get; init; }
    }
}
