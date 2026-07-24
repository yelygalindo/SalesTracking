namespace UrbanTrack.Api.Controllers.Requests.Projects
{
    public sealed class GetProjectsRequest
    {
        public string? Status { get; set; }
        public string? CustomerExternalId { get; set; }
        public string? SellerExternalId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
