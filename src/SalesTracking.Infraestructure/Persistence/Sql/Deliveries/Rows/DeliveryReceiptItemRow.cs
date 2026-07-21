namespace SalesTracking.Infrastructure.Persistence.Sql.Deliveries.Rows
{
    internal sealed class DeliveryReceiptItemRow
    {
        public int Id { get; set; }
        public int DeliveryId { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal DeliveredQuantity { get; set; }
    }
}
