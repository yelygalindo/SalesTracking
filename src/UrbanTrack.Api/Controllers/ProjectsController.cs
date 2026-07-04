using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Interfaces;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.Products;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Projects;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {

        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdMessageResponse>> Create([FromBody] CreateProjectRequest request)
        {
            var result = await _projectService.CreateAsync(request.ToApplication());

            if (result == null)
            {
                return BadRequest(new MessageResponse
                {
                    Message = "No se pudo crear el proyecto. Verifique que el cliente y el vendedor existan."
                });
            }

            return Created(string.Empty, result.ToResponse());
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
    }
}