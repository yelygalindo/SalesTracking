using SalesTracking.Application.UseCases.CustomerTimeline.Comands;
using SalesTracking.Application.UseCases.CustomerTimeline.Results;

namespace SalesTracking.Application.UseCases.CustomerTimeline.Interfaces;

public interface ICustomerTimelineService
{
    Task<GetCustomerTimelineResult> GetAsync(GetCustomerTimelineCommand command);
}
