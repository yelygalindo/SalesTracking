namespace UrbanTrack.Api.Controllers.Requests.Invitations
{
    public class CreateInvitationRequest
    {
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public string InvitedBy { get; set; }
    }
}