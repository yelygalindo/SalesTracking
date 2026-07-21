using SalesTracking.Application.UseCases.Units.Comands;
using SalesTracking.Application.UseCases.Units.Models;
using SalesTracking.Application.UseCases.Units.Results;

namespace SalesTracking.Application.UseCases.Units.Interfaces
{
    public interface IUnitRepository
    {
        Task<UnitPagedList> GetAsync(GetUnitsCommand command);
        Task<UnitResult?> GetByExternalIdAsync(string externalId);
        Task<CreateUnitResult> CreateAsync(CreateUnit unit);
        Task<UpdateUnitResult> UpdateAsync(UpdateUnitCommand command);
        Task<DeleteUnitResult> DeleteAsync(string externalId);
    }
}