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

        [HttpGet("{noteExternalId}")]
        [ProducesResponseType(typeof(ProjectNoteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectNoteResponse>> GetNote(
            string projectExternalId,
            string noteExternalId)
        {
            ProjectNoteResult? note = await _projectNoteService.GetNoteAsync(
                new GetProjectNoteCommand(projectExternalId, noteExternalId));

            if (note == null)
                return NotFound(new ErrorResponse { Error = "Nota de proyecto no encontrada." });

            return Ok(note.ToResponse());
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

        [HttpPut("{noteExternalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> UpdateNote(
            string projectExternalId,
            string noteExternalId,
            [FromBody] UpdateProjectNoteRequest request)
        {
            UpdateProjectNoteResult result = await _projectNoteService.UpdateNoteAsync(
                request.ToApplication(projectExternalId, noteExternalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }

        [HttpDelete("{noteExternalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> DeleteNote(
            string projectExternalId,
            string noteExternalId)
        {
            DeleteProjectNoteResult result = await _projectNoteService.DeleteNoteAsync(
                new DeleteProjectNoteCommand(projectExternalId, noteExternalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }
    }
}