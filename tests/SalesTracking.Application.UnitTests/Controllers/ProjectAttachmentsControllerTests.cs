using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.ProjectAttachments.Interfaces;
using SalesTracking.Application.UseCases.ProjectAttachments.Services;
using UrbanTrack.Api.Controllers;
using UrbanTrack.Api.Controllers.Requests.ProjectAttachments;

namespace SalesTracking.Application.UnitTests.Controllers;

public sealed class ProjectAttachmentsControllerTests
{
    private readonly Mock<IProjectAttachmentRepository> _repository = new();
    private readonly Mock<IFileStorage> _storage = new();
    private readonly Mock<ICurrentUser> _currentUser = new();

    public ProjectAttachmentsControllerTests() => _currentUser.SetupGet(x => x.UserId).Returns(10);

    [Fact]
    public async Task Upload_WhenFileIsTooLarge_ShouldReturn400WithoutSaving()
    {
        ProjectAttachmentsController controller = CreateController();
        FormFile file = new(Stream.Null, 0, 10 * 1024 * 1024 + 1, "file", "photo.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        ActionResult<UrbanTrack.Api.Controllers.Responses.Common.IdMessageResponse> result =
            await controller.Upload("project-1", new UploadProjectAttachmentRequest
            {
                File = file,
                AttachmentType = "Photo"
            });

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _storage.Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
        _repository.Verify(x => x.CreateAsync(It.IsAny<SalesTracking.Application.UseCases.ProjectAttachments.Models.CreateProjectAttachment>()), Times.Never);
    }

    [Fact]
    public async Task Upload_WhenExtensionIsNotAllowed_ShouldReturn400WithoutSaving()
    {
        ProjectAttachmentsController controller = CreateController();
        FormFile file = new(new MemoryStream([1]), 0, 1, "file", "malware.exe")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };

        ActionResult<UrbanTrack.Api.Controllers.Responses.Common.IdMessageResponse> result =
            await controller.Upload("project-1", new UploadProjectAttachmentRequest
            {
                File = file,
                AttachmentType = "Document"
            });

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _storage.Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
    }

    private ProjectAttachmentsController CreateController() => new(
        new ProjectAttachmentService(_repository.Object, _storage.Object),
        _currentUser.Object);
}
