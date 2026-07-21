using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.ProjectMaterials.Comands;
using SalesTracking.Application.UseCases.ProjectMaterials.Interfaces;
using SalesTracking.Application.UseCases.ProjectMaterials.Results;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Mappers;
using UrbanTrack.Api.Controllers.Responses.ProjectMaterials;

namespace UrbanTrack.Api.Controllers;

[ApiController]
[Route("api/projects/{projectExternalId}/materials-summary")]
public sealed class ProjectMaterialsController : ControllerBase
{
    private readonly IProjectMaterialsService _service;

    public ProjectMaterialsController(IProjectMaterialsService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(ProjectMaterialsSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectMaterialsSummaryResponse>> Get(string projectExternalId)
    {
        GetProjectMaterialsSummaryResult result = await _service.GetSummaryAsync(
            new GetProjectMaterialsSummaryCommand(projectExternalId));

        if (!result.Succeeded)
        {
            if (result.NotFound)
                return NotFound(new ErrorResponse { Error = result.Message });

            return BadRequest(new ErrorResponse { Error = result.Message });
        }

        return Ok(result.ToResponse());
    }
}
