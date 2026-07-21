using SalesTracking.Application.UseCases.Dashboard.Comands;
using SalesTracking.Application.UseCases.Dashboard.Results;

namespace SalesTracking.Application.UseCases.Dashboard.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardResult> GetAsync(GetDashboardCommand command);
    }
}
