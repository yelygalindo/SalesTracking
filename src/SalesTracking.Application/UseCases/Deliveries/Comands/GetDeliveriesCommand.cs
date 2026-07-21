namespace SalesTracking.Application.UseCases.Deliveries.Comands
{
    public sealed record GetDeliveriesCommand(int Page, int PageSize);
}