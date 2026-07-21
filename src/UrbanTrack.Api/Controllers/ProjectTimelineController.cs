using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.ProjectTimeline.Interfaces;
using SalesTracking.Application.UseCases.ProjectTimeline.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.ProjectTimeline;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.ProjectTimeline;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/projects/{projectExternalId}/timeline")]
    public sealed class ProjectTimelineController : ControllerBase
    {
        private readonly IProjectTimelineService _projectTimelineService;

        public ProjectTimelineController(IProjectTimelineService projectTimelineService)
        {
            _projectTimelineService = projectTimelineService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProjectTimelineResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<ProjectTimelineResponse>>> Get(
            string projectExternalId,
            [FromQuery] GetProjectTimelineRequest request)
        {
            GetProjectTimelineResult result = await _projectTimelineService.GetAsync(
                request.ToApplication(projectExternalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(result.Timeline.ToResponse());
        }
    }
}