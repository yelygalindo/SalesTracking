namespace SalesTracking.Infrastructure.Persistence.Sql.Reports.Rows
{
    internal sealed class DeliveryReportRow
    {
        public string DeliveryExternalId { get; set; } = string.Empty;
        public string ProjectExternalId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string? CustomerExternalId { get; set; }
        public string? CustomerName { get; set; }
        public string SellerExternalId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CommittedDateUtc { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public int TotalCount { get; set; }
    }
}
