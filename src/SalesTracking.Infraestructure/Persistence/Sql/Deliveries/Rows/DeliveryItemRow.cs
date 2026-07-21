namespace SalesTracking.Infrastructure.Persistence.Sql.Deliveries.Rows
{
    internal sealed class DeliveryItemRow
    {
        public int DeliveryId { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string ProductExternalId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string UnitExternalId { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
    }
}
