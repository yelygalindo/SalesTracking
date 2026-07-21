using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Reports.Interfaces;
using SalesTracking.Application.UseCases.Reports.Models;
using SalesTracking.Application.UseCases.Reports.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.Reports;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Reports;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public sealed class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("deliveries")]
        [ProducesResponseType(typeof(PagedResponse<DeliveryReportResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<DeliveryReportResponse>>> GetDeliveries(
            [FromQuery] GetReportRequest request)
        {
            ReportPagedList<DeliveryReportResult> result = await _reportService.GetDeliveriesAsync(
                request.ToApplication());

            return Ok(result.ToDeliveryResponse());
        }

        [HttpGet("customers-pending-contact")]
        [ProducesResponseType(typeof(PagedResponse<CustomerPendingContactReportResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<CustomerPendingContactReportResponse>>> GetCustomersPendingContact(
            [FromQuery] GetReportRequest request)
        {
            ReportPagedList<CustomerPendingContactReportResult> result = await _reportService.GetCustomersPendingContactAsync(
                request.ToApplication());

            return Ok(result.ToCustomerPendingContactResponse());
        }

        [HttpGet("projects")]
        [ProducesResponseType(typeof(PagedResponse<ProjectReportResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ProjectReportResponse>>> GetProjects(
            [FromQuery] GetReportRequest request)
        {
            ReportPagedList<ProjectReportResult> result = await _reportService.GetProjectsAsync(
                request.ToApplication());

            return Ok(result.ToProjectResponse());
        }

        [HttpGet("commercial-activity")]
        [ProducesResponseType(typeof(PagedResponse<CommercialActivityReportResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<CommercialActivityReportResponse>>> GetCommercialActivity(
            [FromQuery] GetReportRequest request)
        {
            ReportPagedList<CommercialActivityReportResult> result = await _reportService.GetCommercialActivityAsync(
                request.ToApplication());

            return Ok(result.ToCommercialActivityResponse());
        }
    }
}
