using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.ProjectVisits.Interfaces;
using SalesTracking.Application.UseCases.ProjectVisits.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.ProjectVisits;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.ProjectVisits;

namespace UrbanTrack.Api.Controllers;

[ApiController]
[Route("api/projects/{projectExternalId}/visits")]
public sealed class ProjectVisitsController : ControllerBase
{
    private readonly IProjectVisitService _service;
    private readonly ICurrentUser _currentUser;

    public ProjectVisitsController(IProjectVisitService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost]
    [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IdMessageResponse>> Create(
        string projectExternalId,
        [FromBody] CreateProjectVisitRequest request)
    {
        CreateProjectVisitResult result = await _service.CreateAsync(
            request.ToApplication(projectExternalId, _currentUser.UserId.GetValueOrDefault()));

        if (!result.Succeeded)
        {
            if (result.NotFound)
                return NotFound(new ErrorResponse { Error = result.Message });
            return BadRequest(new ErrorResponse { Error = result.Message });
        }

        return Created(string.Empty, new IdMessageResponse { Id = result.Id!, Message = result.Message });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectVisitResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProjectVisitResponse>>> Get(
        string projectExternalId,
        [FromQuery] GetProjectVisitsRequest request)
    {
        GetProjectVisitsResult result = await _service.GetAsync(request.ToApplication(projectExternalId));

        if (!result.Succeeded)
        {
            if (result.NotFound)
                return NotFound(new ErrorResponse { Error = result.Message });
            return BadRequest(new ErrorResponse { Error = result.Message });
        }

        return Ok(result.Visits.Select(x => x.ToResponse()));
    }
}
