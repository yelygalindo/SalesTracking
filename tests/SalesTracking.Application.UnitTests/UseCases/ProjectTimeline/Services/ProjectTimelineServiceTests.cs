using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.ProjectTimeline.Comands;
using SalesTracking.Application.UseCases.ProjectTimeline.Interfaces;
using SalesTracking.Application.UseCases.ProjectTimeline.Results;
using SalesTracking.Application.UseCases.ProjectTimeline.Services;

namespace SalesTracking.Application.UnitTests.UseCases.ProjectTimeline.Services;

public sealed class ProjectTimelineServiceTests
{
    private readonly Mock<IProjectTimelineRepository> _repository = new();

    [Fact]
    public async Task GetAsync_WhenProjectIsBlank_ShouldReturnValidationError()
    {
        ProjectTimelineService service = new(_repository.Object);

        GetProjectTimelineResult result = await service.GetAsync(new GetProjectTimelineCommand(" ", 1, 20));

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("El proyecto es requerido.");
        _repository.Verify(x => x.GetAsync(It.IsAny<GetProjectTimelineCommand>()), Times.Never);
    }

    [Fact]
    public async Task GetAsync_ShouldTrimProjectAndNormalizePagination()
    {
        _repository.Setup(x => x.GetAsync(It.IsAny<GetProjectTimelineCommand>()))
            .ReturnsAsync(new GetProjectTimelineResult());
        ProjectTimelineService service = new(_repository.Object);

        await service.GetAsync(new GetProjectTimelineCommand(" project-1 ", 0, 200));

        _repository.Verify(x => x.GetAsync(It.Is<GetProjectTimelineCommand>(command =>
            command.ProjectExternalId == "project-1" && command.Page == 1 && command.PageSize == 100)), Times.Once);
    }
}
