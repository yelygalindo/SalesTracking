using SalesTracking.Application.UseCases.Reports.Comands;
using SalesTracking.Application.UseCases.Reports.Interfaces;
using SalesTracking.Application.UseCases.Reports.Models;
using SalesTracking.Application.UseCases.Reports.Results;

namespace SalesTracking.Application.UseCases.Reports.Services
{
    public sealed class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public Task<ReportPagedList<DeliveryReportResult>> GetDeliveriesAsync(GetReportCommand command) =>
            _reportRepository.GetDeliveriesAsync(Normalize(command));

        public Task<ReportPagedList<CustomerPendingContactReportResult>> GetCustomersPendingContactAsync(GetReportCommand command) =>
            _reportRepository.GetCustomersPendingContactAsync(Normalize(command));

        public Task<ReportPagedList<ProjectReportResult>> GetProjectsAsync(GetReportCommand command) =>
            _reportRepository.GetProjectsAsync(Normalize(command));

        public Task<ReportPagedList<CommercialActivityReportResult>> GetCommercialActivityAsync(GetReportCommand command) =>
            _reportRepository.GetCommercialActivityAsync(Normalize(command));

        private static GetReportCommand Normalize(GetReportCommand command)
        {
            int page = command.Page <= 0 ? 1 : command.Page;
            int pageSize = command.PageSize <= 0 ? 20 : command.PageSize;

            if (pageSize > 100)
                pageSize = 100;

            return new GetReportCommand(
                command.From,
                command.To,
                string.IsNullOrWhiteSpace(command.SellerExternalId) ? null : command.SellerExternalId.Trim(),
                command.StatusId,
                page,
                pageSize);
        }
    }
}
