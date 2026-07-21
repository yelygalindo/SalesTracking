namespace UrbanTrack.Api.Controllers.Requests.Invitations
{
    public class CreateInvitationRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; } = default!;
        public string RoleCode { get; set; } = default!;
    }
}
