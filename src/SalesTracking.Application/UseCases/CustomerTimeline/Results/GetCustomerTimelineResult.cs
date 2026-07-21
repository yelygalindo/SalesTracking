using SalesTracking.Application.UseCases.CustomerTimeline.Models;

namespace SalesTracking.Application.UseCases.CustomerTimeline.Results;

public sealed class GetCustomerTimelineResult
{
    public bool Succeeded { get; set; }
    public bool NotFound { get; set; }
    public string Message { get; set; } = string.Empty;
    public CustomerTimelinePagedList Timeline { get; set; } = new();
}
