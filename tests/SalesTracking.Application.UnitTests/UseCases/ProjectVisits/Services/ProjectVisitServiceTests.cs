using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.ProjectVisits.Comands;
using SalesTracking.Application.UseCases.ProjectVisits.Interfaces;
using SalesTracking.Application.UseCases.ProjectVisits.Models;
using SalesTracking.Application.UseCases.ProjectVisits.Results;
using SalesTracking.Application.UseCases.ProjectVisits.Services;

namespace SalesTracking.Application.UnitTests.UseCases.ProjectVisits.Services;

public sealed class ProjectVisitServiceTests
{
    private readonly Mock<IProjectVisitRepository> _repository = new();

    [Fact]
    public async Task CreateAsync_WhenSellerIsMissing_ShouldReturnValidationError()
    {
        ProjectVisitService service = new(_repository.Object);

        CreateProjectVisitResult result = await service.CreateAsync(new CreateProjectVisitCommand(
            "project-1", DateTimeOffset.UtcNow, -17.7833m, -63.1821m, null, "RequiresQuotation", 0));

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("El vendedor autenticado es requerido.");
        _repository.Verify(x => x.CreateAsync(It.IsAny<CreateProjectVisit>()), Times.Never);
    }

    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public async Task CreateAsync_WhenCoordinatesAreInvalid_ShouldNotCallRepository(decimal latitude, decimal longitude)
    {
        ProjectVisitService service = new(_repository.Object);

        CreateProjectVisitResult result = await service.CreateAsync(new CreateProjectVisitCommand(
            "project-1", DateTimeOffset.UtcNow, latitude, longitude, null, "RequiresQuotation", 10));

        result.Succeeded.Should().BeFalse();
        _repository.Verify(x => x.CreateAsync(It.IsAny<CreateProjectVisit>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldNormalizeInputAndUseAuthenticatedSeller()
    {
        _repository.Setup(x => x.CreateAsync(It.IsAny<CreateProjectVisit>()))
            .ReturnsAsync(new CreateProjectVisitResult { Succeeded = true });
        ProjectVisitService service = new(_repository.Object);
        DateTimeOffset visitedAt = new(2026, 7, 21, 11, 30, 0, TimeSpan.FromHours(-4));

        await service.CreateAsync(new CreateProjectVisitCommand(
            " project-1 ", visitedAt, -17.7833m, -63.1821m, " revisión ", " RequiresQuotation ", 42));

        _repository.Verify(x => x.CreateAsync(It.Is<CreateProjectVisit>(visit =>
            visit.ProjectExternalId == "project-1" &&
            visit.VisitedAtUtc == visitedAt.UtcDateTime &&
            visit.Notes == "revisión" &&
            visit.Result == "RequiresQuotation" &&
            visit.SellerUserId == 42 &&
            visit.ExternalId.StartsWith("pvisit-"))), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenRangeIsInvalid_ShouldReturnValidationError()
    {
        ProjectVisitService service = new(_repository.Object);

        GetProjectVisitsResult result = await service.GetAsync(new GetProjectVisitsCommand(
            "project-1", null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(-1)));

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("La fecha desde no puede ser posterior a la fecha hasta.");
        _repository.Verify(x => x.GetAsync(It.IsAny<GetProjectVisitsCommand>()), Times.Never);
    }
}
