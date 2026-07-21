namespace SalesTracking.Application.UseCases.Deliveries.Comands
{
    public sealed record DeleteDeliveryCommand(string ExternalId, int DeletedByUserId = 0);
}
