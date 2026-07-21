using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Interfaces;
using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Application.UseCases.Projects.Services;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UnitTests.UseCases.Projects.Services
{
    public sealed class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _repositoryMock = new();
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _service = new ProjectService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAsync_ShouldNormalizeCustomerFilterAndPagination()
        {
            _repositoryMock.Setup(x => x.GetAsync(It.IsAny<GetProjectsCommand>()))
                .ReturnsAsync(new ProjectPagedList());

            await _service.GetAsync(new GetProjectsCommand(
                " Active ", " customer-1 ", " seller-1 ", 0, 500));

            _repositoryMock.Verify(x => x.GetAsync(It.Is<GetProjectsCommand>(command =>
                command.Status == "Active" &&
                command.CustomerId == "customer-1" &&
                command.SellerId == "seller-1" &&
                command.Page == 1 &&
                command.PageSize == 100)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCommandIsValid_ShouldCreateProjectAndCallRepository()
        {
            Project? capturedProject = null;
            int? capturedCreatedByUserId = null;

            _repositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Project>(), It.IsAny<int>()))
                .Callback<Project, int>((project, userId) =>
                {
                    capturedProject = project;
                    capturedCreatedByUserId = userId;
                })
                .ReturnsAsync(new CreateProjectResult
                {
                    Succeeded = true,
                    Id = "prj-created",
                    Message = "Proyecto creado correctamente."
                });

            CreateProjectCommand command = BuildCreateCommand();

            CreateProjectResult? result = await _service.CreateAsync(command);

            result.Should().NotBeNull();
            result!.Succeeded.Should().BeTrue();
            capturedCreatedByUserId.Should().Be(command.CreatedByUserId);
            capturedProject.Should().NotBeNull();
            capturedProject!.ExternalId.Should().StartWith("p-");
            capturedProject.Name.Should().Be("Nuevo proyecto");
            capturedProject.CustomerExternalId.Should().Be(command.CustomerId);
            capturedProject.SellerExternalId.Should().Be(command.SellerId);
            capturedProject.ProgressPercentage.Should().Be(25);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateAsync_WhenNameIsMissing_ShouldReturnValidationError(string name)
        {
            CreateProjectCommand command = BuildCreateCommand(name: name);

            CreateProjectResult? result = await _service.CreateAsync(command);

            result.Should().NotBeNull();
            result!.Succeeded.Should().BeFalse();
            result.Message.Should().Be("El nombre del proyecto es requerido.");
            _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<Project>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenProgressIsOutOfRange_ShouldReturnValidationError()
        {
            CreateProjectCommand command = BuildCreateCommand(progressPercentage: 101);

            CreateProjectResult? result = await _service.CreateAsync(command);

            result.Should().NotBeNull();
            result!.Succeeded.Should().BeFalse();
            result.Message.Should().Be("El porcentaje de avance no es válido.");
            _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<Project>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetAsync_ShouldNormalizePaginationAndCallRepository()
        {
            _repositoryMock
                .Setup(x => x.GetAsync(It.IsAny<GetProjectsCommand>()))
                .ReturnsAsync((GetProjectsCommand command) => new ProjectPagedList
                {
                    Page = command.Page,
                    PageSize = command.PageSize,
                    TotalItems = 0,
                    TotalPages = 0
                });

            ProjectPagedList result = await _service.GetAsync(new GetProjectsCommand(null, null, null, 0, 500));

            result.Page.Should().Be(1);
            result.PageSize.Should().Be(100);
            _repositoryMock.Verify(x => x.GetAsync(
                It.Is<GetProjectsCommand>(command => command.Page == 1 && command.PageSize == 100)),
                Times.Once);
        }

        [Fact]
        public async Task GetByExternalIdAsync_WhenExternalIdIsBlank_ShouldReturnNull()
        {
            ProjectDetailResult? result = await _service.GetByExternalIdAsync(new GetProjectByExternalIdCommand(" "));

            result.Should().BeNull();
            _repositoryMock.Verify(x => x.GetByExternalIdAsync(It.IsAny<GetProjectByExternalIdCommand>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenCommandIsValid_ShouldTrimValuesAndCallRepository()
        {
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateProjectCommand>()))
                .ReturnsAsync(new UpdateProjectResult
                {
                    Succeeded = true,
                    Message = "Proyecto actualizado correctamente."
                });

            UpdateProjectCommand command = BuildUpdateCommand();

            UpdateProjectResult result = await _service.UpdateAsync(command);

            result.Succeeded.Should().BeTrue();
            _repositoryMock.Verify(x => x.UpdateAsync(It.Is<UpdateProjectCommand>(normalized =>
                normalized.ExternalId == "prj-1" &&
                normalized.Name == "Proyecto actualizado" &&
                normalized.CustomerExternalId == "cus-1" &&
                normalized.SellerExternalId == "usr-1" &&
                normalized.UpdatedByUserId == 77)),
                Times.Once);
        }

        [Fact]
        public async Task ChangeStatusAsync_WhenStatusIsInvalid_ShouldReturnValidationError()
        {
            ChangeProjectStatusResult result = await _service.ChangeStatusAsync(new ChangeProjectStatusCommand
            {
                ExternalId = "prj-1",
                StatusId = 0,
                ChangedByUserId = 77
            });

            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("Estado de proyecto inválido.");
            _repositoryMock.Verify(x => x.ChangeStatusAsync(It.IsAny<ChangeProjectStatusCommand>()), Times.Never);
        }

        [Fact]
        public async Task ChangeStatusAsync_WhenCommandIsValid_ShouldTrimExternalIdAndCallRepository()
        {
            _repositoryMock
                .Setup(x => x.ChangeStatusAsync(It.IsAny<ChangeProjectStatusCommand>()))
                .ReturnsAsync(new ChangeProjectStatusResult
                {
                    Succeeded = true,
                    Message = "Estado de proyecto actualizado correctamente."
                });

            ChangeProjectStatusResult result = await _service.ChangeStatusAsync(new ChangeProjectStatusCommand
            {
                ExternalId = " prj-1 ",
                StatusId = 2,
                ChangedByUserId = 77
            });

            result.Succeeded.Should().BeTrue();
            _repositoryMock.Verify(x => x.ChangeStatusAsync(It.Is<ChangeProjectStatusCommand>(command =>
                command.ExternalId == "prj-1" &&
                command.StatusId == 2 &&
                command.ChangedByUserId == 77)),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenExternalIdIsBlank_ShouldReturnValidationError()
        {
            DeleteProjectResult result = await _service.DeleteAsync(new DeleteProjectCommand(" "));

            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("El proyecto es requerido.");
            _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<DeleteProjectCommand>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenCommandIsValid_ShouldTrimExternalIdAndCallRepository()
        {
            _repositoryMock
                .Setup(x => x.DeleteAsync(It.IsAny<DeleteProjectCommand>()))
                .ReturnsAsync(new DeleteProjectResult
                {
                    Succeeded = true,
                    Message = "Proyecto eliminado correctamente."
                });

            DeleteProjectResult result = await _service.DeleteAsync(new DeleteProjectCommand(" prj-1 "));

            result.Succeeded.Should().BeTrue();
            _repositoryMock.Verify(x => x.DeleteAsync(It.Is<DeleteProjectCommand>(command =>
                command.ExternalId == "prj-1")),
                Times.Once);
        }

        private static CreateProjectCommand BuildCreateCommand(
            string name = "Nuevo proyecto",
            decimal? progressPercentage = 25)
        {
            return new CreateProjectCommand(
                name,
                "Descripcion",
                "cus-1",
                "usr-1",
                1000,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30),
                progressPercentage,
                null,
                "Direccion",
                -17.78m,
                -63.18m,
                77);
        }

        private static UpdateProjectCommand BuildUpdateCommand()
        {
            return new UpdateProjectCommand
            {
                ExternalId = " prj-1 ",
                Name = " Proyecto actualizado ",
                Description = " Descripcion ",
                CustomerExternalId = " cus-1 ",
                SellerExternalId = " usr-1 ",
                EstimatedAmount = 5000,
                StartDateUtc = DateTime.UtcNow,
                ExpectedCloseDateUtc = DateTime.UtcNow.AddDays(10),
                ProgressPercentage = 50,
                ActualCloseDateUtc = null,
                Address = " Direccion ",
                Latitude = -17.78m,
                Longitude = -63.18m,
                UpdatedByUserId = 77
            };
        }
    }
}
