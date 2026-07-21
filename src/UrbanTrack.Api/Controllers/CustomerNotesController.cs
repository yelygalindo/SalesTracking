using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.CustomerNotes.Comands;
using SalesTracking.Application.UseCases.CustomerNotes.Interfaces;
using SalesTracking.Application.UseCases.CustomerNotes.Results;
using SalesTracking.Domain.Entities;
using UrbanTrack.Api.Controllers.Requests.CustomerNotes;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.CustomerNotes;
using UrbanTrack.Api.Controllers.Responses.Mappers;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/customers/{customerExternalId}/notes")]
    public class CustomerNotesController : ControllerBase
    {
        private readonly ICustomerNoteService _service;
        private readonly ICurrentUser _currentUser;

        public CustomerNotesController(ICustomerNoteService service, ICurrentUser currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerNoteResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerNoteResponse>>> GetNotes(string customerExternalId)
        {
            IReadOnlyList<CustomerNote> notes = await _service.GetNotesAsync(
                new GetCustomerNotesCommand(customerExternalId));

            return Ok(notes.Select(x => x.ToResponse()));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IdMessageResponse>> AddNote(
            string customerExternalId,
            [FromBody] CustomerNoteRequest request)
        {
            AddCustomerNoteResult result = await _service.AddNoteAsync(request.ToApplication(customerExternalId, _currentUser.UserId));

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

