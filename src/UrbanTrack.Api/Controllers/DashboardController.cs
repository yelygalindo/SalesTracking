using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Dashboard.Interfaces;
using SalesTracking.Application.UseCases.Dashboard.Results;
using UrbanTrack.Api.Controllers.Requests.Dashboard;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.Dashboard;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardResponse>> Get(
            [FromQuery] string? sellerExternalId,
            [FromQuery] int? statusId)
        {
            DashboardResult result = await _dashboardService.GetAsync(
                new GetDashboardRequest
                {
                    SellerExternalId = sellerExternalId,
                    StatusId = statusId
                }.ToApplication());

            return Ok(result.ToResponse());
        }
    }
}