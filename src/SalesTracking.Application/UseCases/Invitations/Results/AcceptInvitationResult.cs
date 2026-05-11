using SalesTracking.Application.Common.Results;

namespace SalesTracking.Application.UseCases.Invitations.Results
{
    public class AcceptInvitationResult: MessageResult
    {
        public string ExternalUserId { get; set; }
    }
}
