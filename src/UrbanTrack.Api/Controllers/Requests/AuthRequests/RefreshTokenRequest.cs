using System.ComponentModel.DataAnnotations;

namespace UrbanTrack.Api.Controllers.Requests.AuthRequests;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
