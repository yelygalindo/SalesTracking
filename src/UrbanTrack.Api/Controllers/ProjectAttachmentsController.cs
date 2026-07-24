using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.ProjectAttachments.Comands;
using SalesTracking.Application.UseCases.ProjectAttachments.Interfaces;
using SalesTracking.Application.UseCases.ProjectAttachments.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.ProjectAttachments;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.ProjectAttachments;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/projects/{projectExternalId}/attachments")]
    public sealed class ProjectAttachmentsController : ControllerBase
    {
        private readonly IProjectAttachmentService _projectAttachmentService;
        private readonly ICurrentUser _currentUser;

        public ProjectAttachmentsController(IProjectAttachmentService projectAttachmentService, ICurrentUser currentUser)
        {
            _projectAttachmentService = projectAttachmentService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectAttachmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProjectAttachmentResponse>>> Get(string projectExternalId)
        {
            GetProjectAttachmentsResult result = await _projectAttachmentService.GetAsync(
                new GetProjectAttachmentsCommand(projectExternalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(result.Items.Select(x => x.ToResponse()));
        }

        [HttpGet("{attachmentExternalId}/content")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContent(
            string projectExternalId,
            string attachmentExternalId)
        {
            ProjectAttachmentContentResult result = await _projectAttachmentService.GetContentAsync(
                new GetProjectAttachmentContentCommand(projectExternalId, attachmentExternalId));

            if (!result.Succeeded || result.Content == null)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return File(result.Content, result.ContentType, result.FileName);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IdMessageResponse>> Upload(
            string projectExternalId,
            [FromForm] UploadProjectAttachmentRequest request)
        {
            UploadProjectAttachmentResult result = await _projectAttachmentService.UploadAsync(
                request.ToApplication(projectExternalId, _currentUser.UserId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return CreatedAtAction(
                nameof(GetContent),
                new
                {
                    projectExternalId,
                    attachmentExternalId = result.Id
                },
                result.ToResponse());
        }

        [HttpDelete("{attachmentExternalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Delete(
            string projectExternalId,
            string attachmentExternalId)
        {
            DeleteProjectAttachmentResult result = await _projectAttachmentService.DeleteAsync(
                new DeleteProjectAttachmentCommand(projectExternalId, attachmentExternalId, _currentUser.UserId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }

        [HttpPut("{attachmentExternalId}/cover")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> SetCover(
            string projectExternalId,
            string attachmentExternalId)
        {
            SetProjectAttachmentCoverResult result = await _projectAttachmentService.SetCoverAsync(
                new SetProjectAttachmentCoverCommand(
                    projectExternalId,
                    attachmentExternalId,
                    _currentUser.UserId));

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
