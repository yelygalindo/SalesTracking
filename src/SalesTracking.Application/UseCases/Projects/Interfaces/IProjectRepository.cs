using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Projects.Interfaces
{
    public interface IProjectRepository
    {
        Task<CreateProjectResult> CreateAsync(Project project, int createdByUserId);
        Task<ProjectPagedList> GetAsync(GetProjectsCommand command);
        Task<ProjectDetailResult?> GetByExternalIdAsync(GetProjectByExternalIdCommand command);
        Task<UpdateProjectResult> UpdateAsync(UpdateProjectCommand command);
        Task<ChangeProjectStatusResult> ChangeStatusAsync(ChangeProjectStatusCommand command);
        Task<DeleteProjectResult> DeleteAsync(DeleteProjectCommand command);
    }
}
