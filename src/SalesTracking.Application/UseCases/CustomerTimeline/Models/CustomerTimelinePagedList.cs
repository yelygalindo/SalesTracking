using SalesTracking.Application.UseCases.CustomerTimeline.Results;

namespace SalesTracking.Application.UseCases.CustomerTimeline.Models;

public sealed class CustomerTimelinePagedList
{
    public IReadOnlyList<CustomerTimelineResult> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
