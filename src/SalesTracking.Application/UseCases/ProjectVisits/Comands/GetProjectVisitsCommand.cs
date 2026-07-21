namespace SalesTracking.Application.UseCases.ProjectVisits.Comands;

public sealed record GetProjectVisitsCommand(
    string ProjectExternalId,
    string? SellerExternalId,
    DateTimeOffset? From,
    DateTimeOffset? To);
