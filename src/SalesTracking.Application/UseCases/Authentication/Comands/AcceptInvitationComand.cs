namespace SalesTracking.Application.UseCases.Authentication.Comands
{
    public class AcceptInvitationComand
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string FullNameUser { get; set; }
    }
}