namespace UrbanTrack.Api.Controllers.Requests.AuthRequests
{
    public class CreateInvitationRequest
    {
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public string InvitedBy { get; set; }
    }
}
