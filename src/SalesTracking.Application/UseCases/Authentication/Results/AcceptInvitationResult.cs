namespace SalesTracking.Application.UseCases.Authentication.Results
{
    public class AcceptInvitationResult: MessageResult
    {
        public string ExternalUserId { get; set; }
    }
}
