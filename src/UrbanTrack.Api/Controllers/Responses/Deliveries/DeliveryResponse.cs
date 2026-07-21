namespace UrbanTrack.Api.Controllers.Responses.Deliveries
{
    public sealed class DeliveryResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public IReadOnlyList<DeliveryItemResponse> Items { get; set; } = new List<DeliveryItemResponse>();
    }
}
