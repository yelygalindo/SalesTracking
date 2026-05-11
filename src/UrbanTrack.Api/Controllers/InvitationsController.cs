using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.Mappers;
using SalesTracking.Application.UseCases.Invitations.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Models;
using SalesTracking.Application.UseCases.Invitations.Results;
using SalesTracking.Application.UseCases.Invitations.Comands;
using UrbanTrack.Api.Controllers.Requests.Invitations;
using UrbanTrack.Api.Controllers.Responses.Invitations;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationsController : ControllerBase
    {
        private readonly IInvitationService _service;

        public InvitationsController(IInvitationService service)
        {
            _service = service;
        }

        [HttpPost("/api/invitations")]
        [ProducesResponseType(typeof(CreateInvitationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateInvitationResponse>> CreateInvitation( [FromBody] CreateInvitationRequest request)
        {
            CreateInvitationResult result =
                await _service.CreateInvitationAsync(request.ToApplication());
            return Ok(result.ToResponse());

        }

        [HttpGet("/api/invitations/{token}")]
        [ProducesResponseType(typeof(InvitationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvitationResponse>> GetInvitationByToken(string token)
        {
            InvitationResult? inv = await _service.GetInvitationByTokenAsync(new GetInvitationByTokenComand(token));
            if (inv == null) return NotFound(new MessageResponse { Message = "Invitación no encontrada." });
            return Ok(inv.ToResponse());
        }

        [HttpPost("/api/invitations/accept")]
        [ProducesResponseType(typeof(AcceptInvitationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<AcceptInvitationResponse>> AcceptInvitation([FromBody] AcceptInvitationInput request)
        {
            AcceptInvitationResult resp = await _service.AcceptInvitationAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
        }
    }
}