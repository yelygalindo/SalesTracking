using SalesTracking.Application.UseCases.Units.Comands;
using SalesTracking.Application.UseCases.Units.Models;
using SalesTracking.Application.UseCases.Units.Results;

namespace SalesTracking.Application.UseCases.Units.Interfaces
{
    public interface IUnitService
    {
        Task<UnitPagedList> GetAsync(GetUnitsCommand command);
        Task<UnitResult?> GetByExternalIdAsync(GetUnitByExternalIdCommand command);
        Task<CreateUnitResult> CreateAsync(CreateUnitCommand command);
        Task<UpdateUnitResult> UpdateAsync(UpdateUnitCommand command);
        Task<DeleteUnitResult> DeleteAsync(DeleteUnitCommand command);
    }
}