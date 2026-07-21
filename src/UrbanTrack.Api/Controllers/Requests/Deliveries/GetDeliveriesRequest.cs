namespace UrbanTrack.Api.Controllers.Requests.Deliveries
{
    public sealed class GetDeliveriesRequest
    {
        public string? ProjectExternalId { get; set; }
        public string? CustomerExternalId { get; set; }
        public string? SellerExternalId { get; set; }
        public int? StatusId { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public bool? Overdue { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
