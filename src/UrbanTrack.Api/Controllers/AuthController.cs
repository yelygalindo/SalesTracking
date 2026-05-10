using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SalesTracking.Application.UseCases.Authentication.Models;
using UrbanTrack.Api.Controllers.Requests.AuthRequests;
using UrbanTrack.Api.Controllers.Responses.AuthResponses;
using SalesTracking.Application.UseCases.Authentication.Results;
using SalesTracking.Application.UseCases.Authentication.Comands;
using UrbanTrack.Api.Controllers.Responses.Common;
using SalesTracking.Application.UseCases.Authentication.Interfaces;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResult resp = await _service.LoginAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<LogoutResponse>> Logout([FromBody] LogoutRequest request)
        {
            LogoutResult resp = await _service.LogoutAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            RefreshTokenResult resp = await _service.RefreshTokenAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            ForgotPasswordResult resp = await _service.ForgotPasswordAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            ResetPasswordResult resp = await _service.ResetPasswordAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
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