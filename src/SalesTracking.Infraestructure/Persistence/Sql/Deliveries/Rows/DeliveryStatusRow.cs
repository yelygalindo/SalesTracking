namespace SalesTracking.Infrastructure.Persistence.Sql.Deliveries.Rows
{
    internal sealed class DeliveryStatusRow
    {
        public int DeliveryStatusId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
