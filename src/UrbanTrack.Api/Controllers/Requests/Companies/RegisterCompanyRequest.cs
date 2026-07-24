namespace UrbanTrack.Api.Controllers.Requests.Companies;

public sealed class RegisterCompanyRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string AdminFullName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
