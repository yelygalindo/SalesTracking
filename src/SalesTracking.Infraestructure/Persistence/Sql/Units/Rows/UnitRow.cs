namespace SalesTracking.Infrastructure.Persistence.Sql.Units.Rows
{
    internal sealed class UnitRow
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool AllowsDecimals { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public int TotalCount { get; set; }
    }
}