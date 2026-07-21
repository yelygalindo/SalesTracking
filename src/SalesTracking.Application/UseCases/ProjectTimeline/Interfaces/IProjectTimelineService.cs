using SalesTracking.Application.UseCases.ProjectTimeline.Comands;
using SalesTracking.Application.UseCases.ProjectTimeline.Results;

namespace SalesTracking.Application.UseCases.ProjectTimeline.Interfaces
{
    public interface IProjectTimelineService
    {
        Task<GetProjectTimelineResult> GetAsync(GetProjectTimelineCommand command);
    }
}