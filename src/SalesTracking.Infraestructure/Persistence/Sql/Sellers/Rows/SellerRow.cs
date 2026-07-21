namespace SalesTracking.Infrastructure.Persistence.Sql.Sellers.Rows;

internal sealed class SellerRow
{
    public string ExternalId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
