using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Comands;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Results;
using UrbanTrack.Api.Controllers;
using UrbanTrack.Api.Controllers.Requests.AuthRequests;

namespace SalesTracking.Application.UnitTests.Controllers;

public sealed class AuthControllerTests
{
    private readonly Mock<IAuthService> _service = new();

    [Fact]
    public async Task Login_WhenCredentialsAreInvalid_ShouldReturn401()
    {
        _service.Setup(x => x.LoginAsync(It.IsAny<LoginCommand>())).ReturnsAsync((LoginResult?)null);
        AuthController controller = new(_service.Object);

        ActionResult result = await controller.Login(new LoginRequest { Email = "bad@example.com", Password = "bad" });

        ObjectResult response = result.Should().BeOfType<ObjectResult>().Subject;
        response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task Me_WhenAuthenticated_ShouldReturnUserId()
    {
        Mock<ICurrentUser> currentUser = new();
        currentUser.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUser.SetupGet(x => x.UserId).Returns(42);
        _service.Setup(x => x.GetMeAsync(42)).ReturnsAsync(new AuthMeResult
        {
            Id = 42,
            ExternalId = "usr-42",
            Username = "user",
            FullName = "User",
            Email = "user@example.com",
            CompanyId = 1,
            CompanyExternalId = "company-1",
            CompanyName = "Company"
        });
        AuthController controller = new(_service.Object, currentUser.Object);

        ActionResult<UrbanTrack.Api.Controllers.Responses.AuthResponses.UserCompleteResponse> result =
            await controller.Me();

        OkObjectResult response = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = response.Value.Should()
            .BeOfType<UrbanTrack.Api.Controllers.Responses.AuthResponses.UserCompleteResponse>().Subject;
        body.Id.Should().Be(42);
        body.Username.Should().Be("user");
    }

    [Fact]
    public async Task RefreshToken_WhenTokenWasAlreadyUsed_ShouldReturn401()
    {
        _service.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenComand>()))
            .ReturnsAsync((RefreshTokenResult?)null);
        AuthController controller = new(_service.Object);

        ActionResult<UrbanTrack.Api.Controllers.Responses.AuthResponses.RefreshTokenResponse> result =
            await controller.RefreshToken(new RefreshTokenRequest { RefreshToken = "already-rotated" });

        ObjectResult response = result.Result.Should().BeOfType<ObjectResult>().Subject;
        response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task ResetPassword_WhenPasswordsDiffer_ShouldReturn400WithoutCallingService()
    {
        AuthController controller = new(_service.Object);

        ActionResult<UrbanTrack.Api.Controllers.Responses.AuthResponses.ResetPasswordResponse> result =
            await controller.ResetPassword(new ResetPasswordRequest
            {
                Token = "token",
                NewPassword = "Password-1",
                ConfirmPassword = "Password-2"
            });

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _service.Verify(x => x.ResetPasswordAsync(It.IsAny<ResetPasswordComand>()), Times.Never);
    }
}
