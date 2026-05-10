namespace SalesTracking.Application.UseCases.Authentication.Comands
{
    public class GetInvitationByTokenComand
    {
        public string Token { get; }
        public GetInvitationByTokenComand(string token)
        {
            Token = token;
        }
    }
}
