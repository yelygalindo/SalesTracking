using SalesTracking.Application.UseCases.CustomerTimeline.Comands;
using SalesTracking.Application.UseCases.CustomerTimeline.Results;

namespace SalesTracking.Application.UseCases.CustomerTimeline.Interfaces;

public interface ICustomerTimelineRepository
{
    Task<GetCustomerTimelineResult> GetAsync(GetCustomerTimelineCommand command);
}
