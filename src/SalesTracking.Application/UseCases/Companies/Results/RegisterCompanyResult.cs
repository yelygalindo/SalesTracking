namespace SalesTracking.Application.UseCases.Companies.Results;

public sealed class RegisterCompanyResult
{
    public bool Succeeded { get; init; }
    public string? CompanyExternalId { get; init; }
    public string? AdminUserExternalId { get; init; }
    public string Message { get; init; } = string.Empty;
}
