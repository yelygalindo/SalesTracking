namespace UrbanTrack.Api.Controllers.Requests.Reports
{
    public sealed class GetReportRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? SellerExternalId { get; set; }
        public int? StatusId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
