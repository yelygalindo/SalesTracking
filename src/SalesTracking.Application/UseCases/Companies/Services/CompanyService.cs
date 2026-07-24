using System.Text.RegularExpressions;
using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Companies.Comands;
using SalesTracking.Application.UseCases.Companies.Interfaces;
using SalesTracking.Application.UseCases.Companies.Models;
using SalesTracking.Application.UseCases.Companies.Results;

namespace SalesTracking.Application.UseCases.Companies.Services;

public sealed class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;
    private readonly IPasswordPolicy _passwordPolicy;

    public CompanyService(ICompanyRepository repository, IPasswordPolicy passwordPolicy)
    {
        _repository = repository;
        _passwordPolicy = passwordPolicy;
    }

    public async Task<RegisterCompanyResult> RegisterAsync(RegisterCompanyCommand command)
    {
        if (command == null ||
            string.IsNullOrWhiteSpace(command.CompanyName) ||
            string.IsNullOrWhiteSpace(command.AdminFullName) ||
            string.IsNullOrWhiteSpace(command.AdminEmail))
        {
            return Failure("La compañía, el nombre y el correo del administrador son requeridos.");
        }

        string email = command.AdminEmail.Trim().ToLowerInvariant();
        if (!System.Net.Mail.MailAddress.TryCreate(email, out _))
            return Failure("El correo del administrador no es válido.");

        var passwordValidation = _passwordPolicy.Validate(command.Password);
        if (!passwordValidation.IsValid)
            return Failure(passwordValidation.Error!);

        string usernameBase = Regex.Replace(
            command.AdminFullName.ToLowerInvariant(),
            @"[^a-z0-9]",
            string.Empty);
        if (string.IsNullOrWhiteSpace(usernameBase))
            usernameBase = "admin";

        return await _repository.RegisterAsync(new RegisterCompany
        {
            CompanyExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.Company),
            CompanyName = command.CompanyName.Trim(),
            AdminUserExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.User),
            AdminFullName = command.AdminFullName.Trim(),
            AdminEmail = email,
            AdminUsername = $"{usernameBase}_{Guid.NewGuid().ToString("N")[..6]}",
            Password = command.Password
        });
    }

    private static RegisterCompanyResult Failure(string message) =>
        new() { Succeeded = false, Message = message };
}
