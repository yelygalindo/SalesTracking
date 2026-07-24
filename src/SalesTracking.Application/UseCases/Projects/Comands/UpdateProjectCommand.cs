namespace SalesTracking.Application.UseCases.Projects.Comands
{
    public sealed class UpdateProjectCommand
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CustomerExternalId { get; set; } = string.Empty;
        public string? SellerExternalId { get; set; }
        public decimal? EstimatedAmount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? ExpectedCloseDateUtc { get; set; }
        public decimal? ProgressPercentage { get; set; }
        public DateTime? ActualCloseDateUtc { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int UpdatedByUserId { get; set; }
    }
}
