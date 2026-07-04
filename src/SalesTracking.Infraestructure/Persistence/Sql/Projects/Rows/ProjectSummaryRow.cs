namespace SalesTracking.Infrastructure.Persistence.Sql.Projects.Rows
{
    internal sealed class ProjectSummaryRow
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CustomerExternalId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string SellerExternalId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal? EstimatedAmount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? ExpectedCloseDateUtc { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateTime? ActualCloseDateUtc { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int TotalCount { get; set; }
    }
}
