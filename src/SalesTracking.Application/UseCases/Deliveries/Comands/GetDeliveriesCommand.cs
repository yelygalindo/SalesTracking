namespace SalesTracking.Application.UseCases.Deliveries.Comands
{
    public sealed record GetDeliveriesCommand(
        int Page,
        int PageSize,
        string? ProjectExternalId = null,
        string? CustomerExternalId = null,
        string? SellerExternalId = null,
        int? StatusId = null,
        DateTimeOffset? From = null,
        DateTimeOffset? To = null,
        bool? Overdue = null);
}
