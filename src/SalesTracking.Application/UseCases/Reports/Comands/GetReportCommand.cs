namespace SalesTracking.Application.UseCases.Reports.Comands
{
    public sealed record GetReportCommand(
        DateTime? From,
        DateTime? To,
        string? SellerExternalId,
        int? StatusId,
        int Page,
        int PageSize);
}
