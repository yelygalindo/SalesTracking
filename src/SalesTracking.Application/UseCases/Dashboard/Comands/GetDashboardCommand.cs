namespace SalesTracking.Application.UseCases.Dashboard.Comands
{
    public sealed record GetDashboardCommand(string? SellerExternalId, int? StatusId);
}
