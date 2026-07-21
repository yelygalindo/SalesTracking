using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.Dashboard.Comands;
using SalesTracking.Application.UseCases.Dashboard.Interfaces;
using SalesTracking.Application.UseCases.Dashboard.Results;
using SalesTracking.Application.UseCases.Dashboard.Services;

namespace SalesTracking.Application.UnitTests.UseCases.Dashboard.Services;

public sealed class DashboardServiceTests
{
    [Theory]
    [InlineData(" usr-1 ", 2, "usr-1", 2)]
    [InlineData(" ", 0, null, null)]
    [InlineData(null, -1, null, null)]
    public async Task GetAsync_ShouldNormalizeFilters(
        string? sellerId, int? statusId, string? expectedSellerId, int? expectedStatusId)
    {
        Mock<IDashboardRepository> repository = new();
        DashboardResult expected = new();
        repository.Setup(x => x.GetAsync(It.IsAny<GetDashboardCommand>())).ReturnsAsync(expected);
        DashboardService service = new(repository.Object);

        DashboardResult result = await service.GetAsync(new GetDashboardCommand(sellerId, statusId));

        result.Should().BeSameAs(expected);
        repository.Verify(x => x.GetAsync(It.Is<GetDashboardCommand>(command =>
            command.SellerExternalId == expectedSellerId && command.StatusId == expectedStatusId)), Times.Once);
    }
}
