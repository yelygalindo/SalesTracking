using SalesTracking.Application.UseCases.Reports.Comands;
using SalesTracking.Application.UseCases.Reports.Models;
using SalesTracking.Application.UseCases.Reports.Results;

namespace SalesTracking.Application.UseCases.Reports.Interfaces
{
    public interface IReportService
    {
        Task<ReportPagedList<DeliveryReportResult>> GetDeliveriesAsync(GetReportCommand command);
        Task<ReportPagedList<CustomerPendingContactReportResult>> GetCustomersPendingContactAsync(GetReportCommand command);
        Task<ReportPagedList<ProjectReportResult>> GetProjectsAsync(GetReportCommand command);
        Task<ReportPagedList<CommercialActivityReportResult>> GetCommercialActivityAsync(GetReportCommand command);
    }
}
