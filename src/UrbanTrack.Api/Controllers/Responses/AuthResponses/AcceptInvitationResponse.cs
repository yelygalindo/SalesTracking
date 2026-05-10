using UrbanTrack.Api.Controllers.Responses.Common;

namespace UrbanTrack.Api.Controllers.Responses.AuthResponses
{
    public class AcceptInvitationResponse : MessageResponse
    {
        public string ExternalUserId { get; set; }
    }
}