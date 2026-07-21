namespace UrbanTrack.Api.Controllers.Requests.Dashboard
{
    public sealed class GetDashboardRequest
    {
        public string? SellerExternalId { get; set; }
        public int? StatusId { get; set; }
    }
}
