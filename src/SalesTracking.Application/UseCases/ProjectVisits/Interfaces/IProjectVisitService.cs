using SalesTracking.Application.UseCases.ProjectVisits.Comands;
using SalesTracking.Application.UseCases.ProjectVisits.Results;

namespace SalesTracking.Application.UseCases.ProjectVisits.Interfaces;

public interface IProjectVisitService
{
    Task<CreateProjectVisitResult> CreateAsync(CreateProjectVisitCommand command);
    Task<GetProjectVisitsResult> GetAsync(GetProjectVisitsCommand command);
}
