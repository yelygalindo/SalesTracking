using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.ProjectMaterials.Comands;
using SalesTracking.Application.UseCases.ProjectMaterials.Interfaces;
using SalesTracking.Application.UseCases.ProjectMaterials.Results;
using SalesTracking.Application.UseCases.ProjectMaterials.Services;

namespace SalesTracking.Application.UnitTests.UseCases.ProjectMaterials.Services;

public sealed class ProjectMaterialsServiceTests
{
    private readonly Mock<IProjectMaterialsRepository> _repository = new();

    [Fact]
    public async Task GetSummaryAsync_WhenProjectIsBlank_ShouldReturnValidationError()
    {
        ProjectMaterialsService service = new(_repository.Object);

        GetProjectMaterialsSummaryResult result = await service.GetSummaryAsync(
            new GetProjectMaterialsSummaryCommand(" "));

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("El proyecto es requerido.");
        _repository.Verify(x => x.GetSummaryAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldTrimProjectExternalId()
    {
        _repository.Setup(x => x.GetSummaryAsync("project-1"))
            .ReturnsAsync(new GetProjectMaterialsSummaryResult { Succeeded = true });
        ProjectMaterialsService service = new(_repository.Object);

        GetProjectMaterialsSummaryResult result = await service.GetSummaryAsync(
            new GetProjectMaterialsSummaryCommand(" project-1 "));

        result.Succeeded.Should().BeTrue();
        _repository.Verify(x => x.GetSummaryAsync("project-1"), Times.Once);
    }
}
