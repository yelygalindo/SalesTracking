namespace SalesTracking.Application.UseCases.Sellers.Results;

public sealed class SellerResult
{
    public string ExternalId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
