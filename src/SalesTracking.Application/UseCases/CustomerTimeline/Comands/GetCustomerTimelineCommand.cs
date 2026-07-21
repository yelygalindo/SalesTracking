namespace SalesTracking.Application.UseCases.CustomerTimeline.Comands;

public sealed record GetCustomerTimelineCommand(string CustomerExternalId, int Page, int PageSize);
