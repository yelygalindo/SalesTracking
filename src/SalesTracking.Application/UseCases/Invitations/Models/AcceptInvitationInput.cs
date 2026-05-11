namespace SalesTracking.Application.UseCases.Invitations.Models
{
    public class AcceptInvitationInput
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string FullNameUser { get; set; }
    }
}