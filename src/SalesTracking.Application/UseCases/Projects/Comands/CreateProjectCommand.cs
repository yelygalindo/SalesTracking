namespace SalesTracking.Application.UseCases.Projects.Comands
{
    public sealed record CreateProjectCommand(
        string Name,
        string? Description,
        string CustomerId,
        string SellerId,
        decimal? EstimatedAmount,
        DateTime? StartDateUtc,
        DateTime? ExpectedCloseDateUtc,
        decimal? ProgressPercentage,
        DateTime? ActualCloseDateUtc,
        string? Address,
        decimal? Latitude,
        decimal? Longitude,
        int CreatedByUserId);
}