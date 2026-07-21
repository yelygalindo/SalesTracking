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
using SalesTracking.Application.Common.Interfaces;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly ICurrentUser? _currentUser;

        public AuthController(IAuthService service, ICurrentUser? currentUser = null)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResult? resp = await _service.LoginAsync(request.ToApplication());
            if (resp == null) return StatusCode(StatusCodes.Status401Unauthorized, new MessageResponse { Message = "Credenciales inválidas." });
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

        [HttpGet("me")]
        [ProducesResponseType(typeof(UserCompleteResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserCompleteResponse>> Me()
        {
            if (_currentUser == null || !_currentUser.IsAuthenticated) return Unauthorized();
            AuthMeResult? result = await _service.GetMeAsync(_currentUser.UserId);
            if (result == null) return NotFound();
            return Ok(new UserCompleteResponse
            {
                ExternalId = result.ExternalId,
                FullName = result.FullName,
                Email = result.Email,
                Company = new CompanyResponse
                {
                    Id = result.CompanyId,
                    ExternalId = result.CompanyExternalId,
                    Name = result.CompanyName
                },
                Roles = result.Roles,
                Permissions = result.Permissions
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            RefreshTokenResult? resp = await _service.RefreshTokenAsync(request.ToApplication());
            if (resp == null) return StatusCode(StatusCodes.Status401Unauthorized, new MessageResponse { Message = "Refresh token inválido o expirado." });
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
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(new MessageResponse { Message = "Las contraseñas no coinciden." });

            ResetPasswordResult resp = await _service.ResetPasswordAsync(request.ToApplication());
            if (!resp.Succeeded)
                return BadRequest(resp.ToResponse());

            return Ok(resp.ToResponse());
        }
    }
}
