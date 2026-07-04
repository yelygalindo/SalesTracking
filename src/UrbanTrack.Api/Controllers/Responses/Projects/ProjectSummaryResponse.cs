namespace UrbanTrack.Api.Controllers.Responses.Projects
{
    public sealed class ProjectSummaryResponse
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string CustomerExternalId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        public string SellerExternalId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal? EstimatedAmount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? ExpectedCloseDateUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
