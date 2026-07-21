using SalesTracking.Application.UseCases.ProjectTimeline.Comands;
using SalesTracking.Application.UseCases.ProjectTimeline.Results;

namespace SalesTracking.Application.UseCases.ProjectTimeline.Interfaces
{
    public interface IProjectTimelineRepository
    {
        Task<GetProjectTimelineResult> GetAsync(GetProjectTimelineCommand command);
    }
}