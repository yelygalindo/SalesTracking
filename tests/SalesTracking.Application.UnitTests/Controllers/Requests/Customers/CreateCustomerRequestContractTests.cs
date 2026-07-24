using System.Text.Json;
using FluentAssertions;
using Moq;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Customers.Interfaces;
using SalesTracking.Application.UseCases.Customers.Services;
using UrbanTrack.Api.Controllers.Requests.Customers;
using UrbanTrack.Api.Controllers.Requests.Mappers;

namespace SalesTracking.Application.UnitTests.Controllers.Requests.Customers;

public sealed class CreateCustomerRequestContractTests
{
    [Fact]
    public async Task PostCustomers_WithOnlyStatusId_ShouldRejectTheRequestAndIgnoreStatusId()
    {
        const string body = """
            {
                "statusId": 3
            }
            """;

        CreateCustomerRequest? request = JsonSerializer.Deserialize<CreateCustomerRequest>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        request.Should().NotBeNull();
        typeof(CreateCustomerRequest)
            .GetProperty("StatusId")
            .Should()
            .BeNull("el estado inicial del cliente se define internamente");

        Mock<ICustomerRepository> repository = new();
        Mock<ICurrentUser> currentUser = new();
        currentUser.SetupGet(x => x.UserExternalId).Returns("current-user");
        currentUser.SetupGet(x => x.Roles).Returns(Array.Empty<string>());
        CustomerService service = new(repository.Object, currentUser.Object);

        var result = await service.CreateCustomerAsync(request!.ToApplication(createdByUserId: 1));

        result.Succeeded.Should().BeFalse();
        repository.Verify(
            x => x.CreateCustomerAsync(It.IsAny<SalesTracking.Application.UseCases.Customers.Models.CreateCustomer>()),
            Times.Never);
    }
}
