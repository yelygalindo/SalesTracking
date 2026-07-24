namespace SalesTracking.Application.UseCases.Companies.Models;

public sealed class RegisterCompany
{
    public string CompanyExternalId { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string AdminUserExternalId { get; init; } = string.Empty;
    public string AdminFullName { get; init; } = string.Empty;
    public string AdminEmail { get; init; } = string.Empty;
    public string AdminUsername { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
