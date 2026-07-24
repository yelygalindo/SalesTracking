namespace SalesTracking.Infrastructure.Persistence.Sql.Deliveries.Rows
{
    internal sealed class DeliveryInternalRow
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int SellerId { get; set; }
        public int StatusId { get; set; }
        public DateTime? DeliveredDateUtc { get; set; }
    }
}
