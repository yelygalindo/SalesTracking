namespace UrbanTrack.Api.Controllers.Responses.Companies;

public sealed class RegisterCompanyResponse
{
    public string CompanyExternalId { get; set; } = string.Empty;
    public string AdminUserExternalId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
