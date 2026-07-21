using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using SalesTracking.Application.UseCases.ProjectNotes.Interfaces;
using SalesTracking.Application.UseCases.ProjectNotes.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.ProjectNotes;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Mappers;
using UrbanTrack.Api.Controllers.Responses.Projects;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/projects/{projectExternalId}/notes")]
    public class ProjectNotesController : ControllerBase
    {
        private readonly IProjectNoteService _projectNoteService;

        public ProjectNotesController(IProjectNoteService projectNoteService)
        {
            _projectNoteService = projectNoteService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectNoteResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProjectNoteResponse>>> GetNotes(string projectExternalId)
        {
            IReadOnlyList<ProjectNoteResult> notes = await _projectNoteService.GetNotesAsync(
                new GetProjectNotesCommand(projectExternalId));

            return Ok(notes.Select(x => x.ToResponse()));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IdMessageResponse>> AddNote(
            string projectExternalId,
            [FromBody] ProjectNoteRequest request)
        {
            AddProjectNoteResult result = await _projectNoteService.AddNoteAsync(
                request.ToApplication(projectExternalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Created(string.Empty, result.ToResponse());
        }
    }
}
