namespace SalesTracking.Application.UseCases.Units.Comands
{
    public sealed record GetUnitsCommand(string? Search, int Page, int PageSize);
}