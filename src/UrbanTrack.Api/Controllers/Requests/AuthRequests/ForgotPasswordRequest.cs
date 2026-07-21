using System.ComponentModel.DataAnnotations;

namespace UrbanTrack.Api.Controllers.Requests.AuthRequests;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
