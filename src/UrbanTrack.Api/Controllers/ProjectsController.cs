using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Interfaces;
using SalesTracking.Application.UseCases.Projects.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.Products;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Projects;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {

        private readonly IProjectService _projectService;
        private readonly ICurrentUser _currentUser;

        public ProjectsController(IProjectService projectService, ICurrentUser currentUser)
        {
            _projectService = projectService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectDetailResponse>> Create([FromBody] CreateProjectRequest request)
        {
            var result = await _projectService.CreateAsync(request.ToApplication(_currentUser.UserId.GetValueOrDefault()));

            if (result == null || !result.Succeeded)
                return BadRequest(new MessageResponse { Message = result?.Message ?? "No se pudo crear el proyecto." });

            if (result.Project == null)
                return BadRequest(new MessageResponse { Message = "No se pudo obtener el proyecto creado." });

            return CreatedAtAction(
                nameof(GetByExternalId),
                new { externalId = result.Project.ExternalId },
                result.Project.ToResponse());
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProjectSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ProjectSummaryResponse>>> Get(
            [FromQuery] string? status,
            [FromQuery] string? customerId,
            [FromQuery] string? sellerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var request = new GetProjectsRequest
            {
                Status = status,
                CustomerExternalId = customerId,
                SellerExternalId = sellerId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _projectService.GetAsync(request.ToApplication());

            return Ok(result.ToResponse());
        }

        [HttpGet("{externalId}")]
        [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDetailResponse>> GetByExternalId(string externalId)
        {
            var result = await _projectService.GetByExternalIdAsync(
                new GetProjectByExternalIdCommand(externalId));

            if (result == null)
                return NotFound(new ErrorResponse { Error = "Proyecto no encontrado." });

            return Ok(result.ToResponse());
        }

        [HttpPut("{externalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Update(
            string externalId,
            [FromBody] UpdateProjectRequest request)
        {
            UpdateProjectResult result = await _projectService.UpdateAsync(
                request.ToApplication(externalId, _currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new MessageResponse { Message = result.Message });

                return BadRequest(new MessageResponse { Message = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }

        [HttpPatch("{externalId}/status")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> ChangeStatus(
            string externalId,
            [FromBody] ChangeProjectStatusRequest request)
        {
            ChangeProjectStatusResult result = await _projectService.ChangeStatusAsync(
                request.ToApplication(externalId, _currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new MessageResponse { Message = result.Message });

                return BadRequest(new MessageResponse { Message = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }

        [HttpDelete("{externalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Delete(string externalId)
        {
            DeleteProjectResult result = await _projectService.DeleteAsync(
                new DeleteProjectCommand(externalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new MessageResponse { Message = result.Message });

                return BadRequest(new MessageResponse { Message = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }
    }
}
