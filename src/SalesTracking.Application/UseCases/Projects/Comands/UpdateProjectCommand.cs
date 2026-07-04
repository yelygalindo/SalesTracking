namespace SalesTracking.Application.UseCases.Projects.Comands
{
    public sealed class UpdateProjectCommand
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CustomerExternalId { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public decimal? EstimatedAmount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? ExpectedCloseDateUtc { get; set; }
    }
}
