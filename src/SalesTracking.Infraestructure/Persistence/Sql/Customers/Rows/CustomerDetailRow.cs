namespace SalesTracking.Infrastructure.Persistence.Sql.Customers.Rows
{
    public class CustomerDetailRow
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
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
