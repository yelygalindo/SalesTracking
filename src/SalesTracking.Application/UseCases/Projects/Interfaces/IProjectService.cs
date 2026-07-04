using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Projects.Interfaces
{
    public interface IProjectService
    {
        Task<CreateProjectResult?> CreateAsync(CreateProjectCommand command);
        Task<ProjectPagedList> GetAsync(GetProjectsCommand command);
        Task<ProjectDetailResult?> GetByExternalIdAsync(GetProjectByExternalIdCommand command);
    }
}
