using FluentAssertions;
using Moq;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.Common.Validation;
using SalesTracking.Application.UseCases.Companies.Comands;
using SalesTracking.Application.UseCases.Companies.Interfaces;
using SalesTracking.Application.UseCases.Companies.Models;
using SalesTracking.Application.UseCases.Companies.Results;
using SalesTracking.Application.UseCases.Companies.Services;

namespace SalesTracking.Application.UnitTests.UseCases.Companies.Services;

public sealed class CompanyServiceTests
{
    [Fact]
    public async Task RegisterAsync_WhenInputIsValid_ShouldNormalizeAndCallRepository()
    {
        Mock<ICompanyRepository> repository = new();
        repository.Setup(x => x.RegisterAsync(It.IsAny<RegisterCompany>()))
            .ReturnsAsync(new RegisterCompanyResult
            {
                Succeeded = true,
                CompanyExternalId = "company-1",
                AdminUserExternalId = "user-1"
            });
        IPasswordPolicy passwordPolicy = new PasswordPolicy();
        CompanyService service = new(repository.Object, passwordPolicy);

        RegisterCompanyResult result = await service.RegisterAsync(
            new RegisterCompanyCommand(
                " Company ",
                " Admin User ",
                " ADMIN@EXAMPLE.COM ",
                "Password1"));

        result.Succeeded.Should().BeTrue();
        repository.Verify(x => x.RegisterAsync(It.Is<RegisterCompany>(company =>
            company.CompanyName == "Company" &&
            company.AdminFullName == "Admin User" &&
            company.AdminEmail == "admin@example.com")), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenPasswordIsInvalid_ShouldNotCallRepository()
    {
        Mock<ICompanyRepository> repository = new();
        CompanyService service = new(repository.Object, new PasswordPolicy());

        RegisterCompanyResult result = await service.RegisterAsync(
            new RegisterCompanyCommand("Company", "Admin", "admin@example.com", "short"));

        result.Succeeded.Should().BeFalse();
        repository.Verify(x => x.RegisterAsync(It.IsAny<RegisterCompany>()), Times.Never);
    }
}
