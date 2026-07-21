using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Results;
using UrbanTrack.Api.Controllers.Requests.AuthRequests;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.AuthResponses;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Mappers;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            RefreshTokenResult resp = await _service.RefreshTokenAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            ForgotPasswordResult resp = await _service.ForgotPasswordAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            ResetPasswordResult resp = await _service.ResetPasswordAsync(request.ToApplication());
            if (resp == null) return NotFound(new MessageResponse { Message = "Usuario no encontrado." });
            return Ok(resp.ToResponse());
        }
    }
}