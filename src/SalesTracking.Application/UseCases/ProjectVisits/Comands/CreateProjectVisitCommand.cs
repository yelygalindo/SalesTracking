namespace SalesTracking.Application.UseCases.ProjectVisits.Comands;

public sealed record CreateProjectVisitCommand(
    string ProjectExternalId,
    DateTimeOffset VisitedAtUtc,
    decimal Latitude,
    decimal Longitude,
    string? Notes,
    string Result,
    int SellerUserId);
