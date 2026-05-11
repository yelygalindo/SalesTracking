namespace SalesTracking.Application.UseCases.Invitations.Comands
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
