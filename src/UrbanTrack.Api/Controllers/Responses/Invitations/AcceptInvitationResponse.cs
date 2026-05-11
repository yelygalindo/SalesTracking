using UrbanTrack.Api.Controllers.Responses.Common;

namespace UrbanTrack.Api.Controllers.Responses.Invitations
{
    public class AcceptInvitationResponse : MessageResponse
    {
        public string ExternalUserId { get; set; }
    }
}