using FluentAssertions;
using Moq;
using SalesTracking.Application.UseCases.Authentication.Comands;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Models;
using SalesTracking.Application.UseCases.Authentication.Results;
using SalesTracking.Application.UseCases.Authentication.Services;

namespace SalesTracking.Application.UnitTests.UseCases.Authentication.Services;

public sealed class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _repository = new();

    [Fact]
    public async Task RefreshTokenAsync_WhenTokenIsBlank_ShouldNotCallRepository()
    {
        AuthService service = new(_repository.Object);

        RefreshTokenResult? result = await service.RefreshTokenAsync(new RefreshTokenComand { RefreshToken = " " });

        result.Should().BeNull();
        _repository.Verify(x => x.RefreshTokensAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenValid_ShouldTrimTokenAndMapResult()
    {
        DateTime expiration = DateTime.UtcNow.AddHours(1);
        _repository.Setup(x => x.RefreshTokensAsync("refresh-token"))
            .ReturnsAsync(new AuthTokens
            {
                AccessToken = "access-token",
                RefreshToken = "new-refresh-token",
                ExpiresAtUtc = expiration
            });
        AuthService service = new(_repository.Object);

        RefreshTokenResult? result = await service.RefreshTokenAsync(
            new RefreshTokenComand { RefreshToken = " refresh-token " });

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
        result.ExpiresAtUtc.Should().Be(expiration);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldAlwaysReturnGenericMessageWithoutToken()
    {
        _repository.Setup(x => x.SendForgotPasswordAsync("user@example.com"))
            .ReturnsAsync(new PasswordForgot { Email = "user@example.com", Token = "secret-token" });
        AuthService service = new(_repository.Object);

        ForgotPasswordResult result = await service.ForgotPasswordAsync(
            new ForgotPasswordComand { Email = " user@example.com " });

        result.Message.Should().Be("Si el correo está registrado, recibirás instrucciones para restablecer tu contraseña.");
        result.Message.Should().NotContain("secret-token");
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenPasswordIsTooShort_ShouldNotCallRepository()
    {
        AuthService service = new(_repository.Object);

        ResetPasswordResult result = await service.ResetPasswordAsync(new ResetPasswordComand
        {
            Token = "token", NewPassword = "short"
        });

        result.Message.Should().Be("La contraseña debe tener al menos 8 caracteres");
        _repository.Verify(x => x.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
