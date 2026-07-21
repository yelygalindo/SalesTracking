namespace SalesTracking.Infrastructure.Persistence.Sql.Dashboard.Rows
{
    internal sealed class DashboardProjectLocationRow
    {
        public string ProjectExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal ProgressPercentage { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
