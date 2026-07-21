using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.CustomerTimeline.Interfaces;
using SalesTracking.Application.UseCases.CustomerTimeline.Results;
using UrbanTrack.Api.Controllers.Requests.CustomerTimeline;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.CustomerTimeline;
using UrbanTrack.Api.Controllers.Responses.Pagination;

namespace UrbanTrack.Api.Controllers;

[ApiController]
[Route("api/customers/{customerExternalId}/timeline")]
public sealed class CustomerTimelineController : ControllerBase
{
    private readonly ICustomerTimelineService _service;

    public CustomerTimelineController(ICustomerTimelineService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CustomerTimelineResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<CustomerTimelineResponse>>> Get(
        string customerExternalId,
        [FromQuery] GetCustomerTimelineRequest request)
    {
        GetCustomerTimelineResult result = await _service.GetAsync(
            request.ToApplication(customerExternalId));

        if (!result.Succeeded)
        {
            ErrorResponse error = new() { Error = result.Message };
            return result.NotFound ? NotFound(error) : BadRequest(error);
        }

        return Ok(result.Timeline.ToResponse());
    }
}
