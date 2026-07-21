using SalesTracking.Application.UseCases.ProjectMaterials.Results;

namespace SalesTracking.Application.UseCases.ProjectMaterials.Interfaces;

public interface IProjectMaterialsRepository
{
    Task<GetProjectMaterialsSummaryResult> GetSummaryAsync(string projectExternalId);
}
