namespace UrbanTrack.Api.Controllers.Requests.Products
{
    public sealed class CreateProjectRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CustomerExternalId { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public decimal? EstimatedAmount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? ExpectedCloseDateUtc { get; set; }
    }
}