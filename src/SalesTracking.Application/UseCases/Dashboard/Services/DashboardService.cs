using SalesTracking.Application.UseCases.Dashboard.Comands;
using SalesTracking.Application.UseCases.Dashboard.Interfaces;
using SalesTracking.Application.UseCases.Dashboard.Results;

namespace SalesTracking.Application.UseCases.Dashboard.Services
{
    public sealed class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardResult> GetAsync(GetDashboardCommand command)
        {
            string? sellerExternalId = string.IsNullOrWhiteSpace(command.SellerExternalId)
                ? null
                : command.SellerExternalId.Trim();

            int? statusId = command.StatusId <= 0 ? null : command.StatusId;

            return await _dashboardRepository.GetAsync(new GetDashboardCommand(sellerExternalId, statusId));
        }
    }
}
