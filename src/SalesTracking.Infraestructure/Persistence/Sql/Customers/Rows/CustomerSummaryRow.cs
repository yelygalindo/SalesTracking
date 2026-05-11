namespace SalesTracking.Infrastructure.Persistence.Sql.Customers.Rows
{
    public class CustomerSummaryRow
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int StatusId { get; set; }
        public int SellerId { get; set; }
        public string ExternalSellerId { get; set; }
        public string SellerUserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
