using SalesTracking.Application.UseCases.ProjectTimeline.Comands;
using SalesTracking.Application.UseCases.ProjectTimeline.Interfaces;
using SalesTracking.Application.UseCases.ProjectTimeline.Results;

namespace SalesTracking.Application.UseCases.ProjectTimeline.Services
{
    public sealed class ProjectTimelineService : IProjectTimelineService
    {
        private readonly IProjectTimelineRepository _projectTimelineRepository;

        public ProjectTimelineService(IProjectTimelineRepository projectTimelineRepository)
        {
            _projectTimelineRepository = projectTimelineRepository;
        }

        public async Task<GetProjectTimelineResult> GetAsync(GetProjectTimelineCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ProjectExternalId))
            {
                return new GetProjectTimelineResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            int page = command.Page <= 0 ? 1 : command.Page;
            int pageSize = command.PageSize <= 0 ? 20 : command.PageSize;

            if (pageSize > 100)
                pageSize = 100;

            return await _projectTimelineRepository.GetAsync(
                new GetProjectTimelineCommand(
                    command.ProjectExternalId.Trim(),
                    page,
                    pageSize));
        }
    }
}