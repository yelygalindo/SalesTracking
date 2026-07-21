using SalesTracking.Application.UseCases.ProjectMaterials.Comands;
using SalesTracking.Application.UseCases.ProjectMaterials.Results;

namespace SalesTracking.Application.UseCases.ProjectMaterials.Interfaces;

public interface IProjectMaterialsService
{
    Task<GetProjectMaterialsSummaryResult> GetSummaryAsync(GetProjectMaterialsSummaryCommand command);
}
