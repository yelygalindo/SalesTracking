using Microsoft.AspNetCore.Mvc;
using UrbanTrack.Api.Controllers.Requests.AuthRequests;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            return Ok(new LoginResponse
            {
                AccessToken = "mock-jwt-access-token",
                RefreshToken = "mock-jwt-refresh-token",
                ExpiresIn = 3600,
                User = new AuthUserDto
                {
                    Id = "user-001",
                    FullName = "Carlos Rojas",
                    Email = request.Email,
                    Role = "Salesperson",
                    Status = "Active",
                    Company = new CompanyDto
                    {
                        Id = "company-001",
                        Name = "UrbanTrack Demo Company"
                    },
                    Permissions =
    
                    [
                        "customers.read",
                        "customers.create",
                        "projects.read",
                        "projects.create",
                        "deliveries.read",
                        "routes.view",
                        "reports.view"
                    ]
                }
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequest request)
        {
            return Ok(new
            {
                message = "Session closed successfully."
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            return Ok(new
            {
                accessToken = "new-mock-jwt-access-token",
                refreshToken = "new-mock-jwt-refresh-token",
                expiresIn = 3600
            });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            return Ok(new
            {
                message = "If the email exists, password reset instructions were sent."
            });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            return Ok(new
            {
                message = "Password updated successfully."
            });
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new AuthUserDto
            {
                Id = "user-001",
                FullName = "Carlos Rojas",
                Email = "carlos@urbantrack.com",
                Role = "Salesperson",
                Status = "Active",
                Company = new CompanyDto
                {
                    Id = "company-001",
                    Name = "UrbanTrack Demo Company"
                },
                Permissions =
    
                [
                    "customers.read",
                    "customers.create",
                    "projects.read",
                    "projects.create",
                    "deliveries.read",
                    "routes.view",
                    "reports.view"
                ]
            });
        }

        [HttpGet("/api/invitations/{token}")]
        public IActionResult GetInvitation(string token)
        {
            return Ok(new
            {
                invitationToken = token,
                fullName = "María Pérez",
                email = "maria@urbantrack.com",
                role = "Administrator",
                company = new CompanyDto
                {
                    Id = "company-001",
                    Name = "UrbanTrack Demo Company"
                },
                expiresAt = DateTime.UtcNow.AddDays(2)
            });
        }

        [HttpPost("/api/invitations/accept")]
        public IActionResult AcceptInvitation([FromBody] AcceptInvitationRequest request)
        {
            return Ok(new
            {
                message = "Account activated successfully.",
                user = new AuthUserDto
                {
                    Id = "user-002",
                    FullName = "María Pérez",
                    Email = "maria@urbantrack.com",
                    Role = "Administrator",
                    Status = "Active",
                    Company = new CompanyDto
                    {
                        Id = "company-001",
                        Name = "UrbanTrack Demo Company"
                    },
                    Permissions =
    
                    [
                        "users.read",
                        "users.create",
                        "customers.read",
                        "projects.create",
                        "routes.view_all",
                        "reports.view"
                    ]
                }
            });
        }
    }
}
