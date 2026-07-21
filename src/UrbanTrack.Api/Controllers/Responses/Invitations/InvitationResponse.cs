namespace UrbanTrack.Api.Controllers.Responses.Invitations
{
    public class InvitationResponse
    {
        public string Email { get; set; }
        public string FullName { get; set; } = default!;
        public string RoleCode { get; set; } = default!;
        public string InvitedBy { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}
