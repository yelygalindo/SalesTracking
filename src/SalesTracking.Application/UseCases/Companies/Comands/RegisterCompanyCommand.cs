namespace SalesTracking.Application.UseCases.Companies.Comands;

public sealed record RegisterCompanyCommand(
    string CompanyName,
    string AdminFullName,
    string AdminEmail,
    string Password);
