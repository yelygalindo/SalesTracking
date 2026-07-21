using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.Units.Comands;
using SalesTracking.Application.UseCases.Units.Interfaces;
using SalesTracking.Application.UseCases.Units.Models;
using SalesTracking.Application.UseCases.Units.Results;
using SalesTracking.Application.UseCases.Units.Services;

namespace SalesTracking.Application.UnitTests.UseCases.Units.Services;

public sealed class UnitServiceTests
{
    private readonly Mock<IUnitRepository> _repository = new();

    [Fact]
    public async Task GetAsync_ShouldNormalizeSearchAndPagination()
    {
        _repository.Setup(x => x.GetAsync(It.IsAny<GetUnitsCommand>())).ReturnsAsync(new UnitPagedList());
        UnitService service = new(_repository.Object);

        await service.GetAsync(new GetUnitsCommand(" cement ", 0, 300));

        _repository.Verify(x => x.GetAsync(It.Is<GetUnitsCommand>(command =>
            command.Search == "cement" && command.Page == 1 && command.PageSize == 100)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenNameIsBlank_ShouldNotCallRepository()
    {
        UnitService service = new(_repository.Object);

        CreateUnitResult result = await service.CreateAsync(new CreateUnitCommand
        {
            Name = " ",
            Symbol = "kg"
        });

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("El nombre de la unidad es requerido.");
        _repository.Verify(x => x.CreateAsync(It.IsAny<CreateUnit>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldTrimValuesAndGenerateExternalId()
    {
        _repository.Setup(x => x.CreateAsync(It.IsAny<CreateUnit>()))
            .ReturnsAsync(new CreateUnitResult { Succeeded = true });
        UnitService service = new(_repository.Object);

        await service.CreateAsync(new CreateUnitCommand
        {
            Name = " Kilogramo ", Symbol = " kg ", Description = " Peso ", IsActive = true
        });

        _repository.Verify(x => x.CreateAsync(It.Is<CreateUnit>(unit =>
            unit.ExternalId.StartsWith("unit-") && unit.Name == "Kilogramo" &&
            unit.Symbol == "kg" && unit.Description == "Peso")), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsBlank_ShouldNotCallRepository()
    {
        UnitService service = new(_repository.Object);

        DeleteUnitResult result = await service.DeleteAsync(new DeleteUnitCommand(" "));

        result.Succeeded.Should().BeFalse();
        _repository.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Never);
    }
}
