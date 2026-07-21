using SalesTracking.Application.UseCases.ProjectVisits.Comands;
using SalesTracking.Application.UseCases.ProjectVisits.Models;
using SalesTracking.Application.UseCases.ProjectVisits.Results;

namespace SalesTracking.Application.UseCases.ProjectVisits.Interfaces;

public interface IProjectVisitRepository
{
    Task<CreateProjectVisitResult> CreateAsync(CreateProjectVisit visit);
    Task<GetProjectVisitsResult> GetAsync(GetProjectVisitsCommand command);
}
