using SalesTracking.Application.UseCases.ProjectMaterials.Comands;
using SalesTracking.Application.UseCases.ProjectMaterials.Interfaces;
using SalesTracking.Application.UseCases.ProjectMaterials.Results;

namespace SalesTracking.Application.UseCases.ProjectMaterials.Services;

public sealed class ProjectMaterialsService : IProjectMaterialsService
{
    private readonly IProjectMaterialsRepository _repository;

    public ProjectMaterialsService(IProjectMaterialsRepository repository) => _repository = repository;

    public Task<GetProjectMaterialsSummaryResult> GetSummaryAsync(GetProjectMaterialsSummaryCommand command)
    {
        if (command == null || string.IsNullOrWhiteSpace(command.ProjectExternalId))
        {
            return Task.FromResult(new GetProjectMaterialsSummaryResult
            {
                Message = "El proyecto es requerido."
            });
        }

        return _repository.GetSummaryAsync(command.ProjectExternalId.Trim());
    }
}
