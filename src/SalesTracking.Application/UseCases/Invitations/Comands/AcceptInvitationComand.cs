namespace SalesTracking.Application.UseCases.Invitations.Comands
{
    public class AcceptInvitationComand
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string FullNameUser { get; set; }
    }
}