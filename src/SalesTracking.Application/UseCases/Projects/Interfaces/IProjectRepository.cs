using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Projects.Interfaces
{
    public interface IProjectRepository
    {
        Task<string?> CreateAsync(Project project);
        Task<ProjectPagedList> GetAsync(GetProjectsCommand command);
        Task<ProjectDetailResult?> GetByExternalIdAsync(GetProjectByExternalIdCommand command);
    }
}
