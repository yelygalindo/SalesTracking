using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.Reports.Comands;
using SalesTracking.Application.UseCases.Reports.Interfaces;
using SalesTracking.Application.UseCases.Reports.Models;
using SalesTracking.Application.UseCases.Reports.Results;
using SalesTracking.Application.UseCases.Reports.Services;

namespace SalesTracking.Application.UnitTests.UseCases.Reports.Services;

public sealed class ReportServiceTests
{
    [Fact]
    public async Task GetProjectsAsync_ShouldNormalizePaginationAndSeller()
    {
        Mock<IReportRepository> repository = new();
        ReportPagedList<ProjectReportResult> expected = new();
        repository.Setup(x => x.GetProjectsAsync(It.IsAny<GetReportCommand>())).ReturnsAsync(expected);
        ReportService service = new(repository.Object);

        ReportPagedList<ProjectReportResult> result = await service.GetProjectsAsync(
            new GetReportCommand(null, null, " seller-1 ", null, 0, 500));

        result.Should().BeSameAs(expected);
        repository.Verify(x => x.GetProjectsAsync(It.Is<GetReportCommand>(command =>
            command.SellerExternalId == "seller-1" && command.Page == 1 && command.PageSize == 100)), Times.Once);
    }

    [Fact]
    public async Task GetDeliveriesAsync_WhenSellerIsBlank_ShouldSendNullSellerAndDefaultPageSize()
    {
        Mock<IReportRepository> repository = new();
        repository.Setup(x => x.GetDeliveriesAsync(It.IsAny<GetReportCommand>()))
            .ReturnsAsync(new ReportPagedList<DeliveryReportResult>());
        ReportService service = new(repository.Object);

        await service.GetDeliveriesAsync(new GetReportCommand(null, null, " ", null, -1, 0));

        repository.Verify(x => x.GetDeliveriesAsync(It.Is<GetReportCommand>(command =>
            command.SellerExternalId == null && command.Page == 1 && command.PageSize == 20)), Times.Once);
    }
}
