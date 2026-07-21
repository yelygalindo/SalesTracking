using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.CustomerTimeline.Comands;
using SalesTracking.Application.UseCases.CustomerTimeline.Interfaces;
using SalesTracking.Application.UseCases.CustomerTimeline.Results;
using SalesTracking.Application.UseCases.CustomerTimeline.Services;

namespace SalesTracking.Application.UnitTests.UseCases.CustomerTimeline.Services;

public sealed class CustomerTimelineServiceTests
{
    private readonly Mock<ICustomerTimelineRepository> _repository = new();

    [Fact]
    public async Task GetAsync_WhenCustomerIsBlank_ShouldReturnValidationError()
    {
        CustomerTimelineService service = new(_repository.Object);

        GetCustomerTimelineResult result = await service.GetAsync(new GetCustomerTimelineCommand(" ", 1, 20));

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("El cliente es requerido.");
        _repository.Verify(x => x.GetAsync(It.IsAny<GetCustomerTimelineCommand>()), Times.Never);
    }

    [Fact]
    public async Task GetAsync_ShouldTrimCustomerAndNormalizePagination()
    {
        _repository.Setup(x => x.GetAsync(It.IsAny<GetCustomerTimelineCommand>()))
            .ReturnsAsync(new GetCustomerTimelineResult { Succeeded = true });
        CustomerTimelineService service = new(_repository.Object);

        await service.GetAsync(new GetCustomerTimelineCommand(" customer-1 ", 0, 500));

        _repository.Verify(x => x.GetAsync(It.Is<GetCustomerTimelineCommand>(command =>
            command.CustomerExternalId == "customer-1" && command.Page == 1 && command.PageSize == 100)),
            Times.Once);
    }
}
