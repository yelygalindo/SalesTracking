using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanTrack.Api.Controllers.Requests.Customers;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Customers;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerNotesController : ControllerBase
    {
        [HttpGet("{id}/notes")]
        [ProducesResponseType(typeof(IEnumerable<CustomerNoteResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerNoteResponse>>> GetNotes(string id)
        {
            var notes = new List<CustomerNoteResponse>
            {
                new CustomerNoteResponse { Text = "Cotizar 200 m2 de Aislantes.", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new CustomerNoteResponse { Text = "Confirmar entrega de clavijas.", CreatedAt = DateTime.UtcNow.AddDays(-1) }
            };

            return await Task.FromResult(Ok(notes));
        }

        [HttpPost("{id}/notes")]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<IdMessageResponse>> AddNote(string id, [FromBody] CustomerNoteRequest req)
        {
            var noteId = $"n-{Guid.NewGuid():N}".Substring(0, 8);
            return await Task.FromResult(Created(string.Empty, new IdMessageResponse { Id = noteId, Message = "Nota agregada (mock)." }));
        }
    }
}
