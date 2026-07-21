using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SalesTracking.Application.UnitTests.Security;

public sealed class JwtSecurityTests
{
    [Fact]
    public async Task AuthenticatedUserPolicy_WhenThereIsNoToken_ShouldRejectTheRequest()
    {
        AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        AuthorizationHandlerContext context = new(
            policy.Requirements,
            new ClaimsPrincipal(new ClaimsIdentity()),
            null);

        foreach (IAuthorizationRequirement requirement in policy.Requirements)
        {
            if (requirement is IAuthorizationHandler handler)
                await handler.HandleAsync(context);
        }

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public void ValidateToken_WhenTokenIsExpired_ShouldRejectIt()
    {
        const string secret = "test-secret-with-at-least-32-characters";
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secret));
        JwtSecurityToken token = new(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: [new Claim(JwtRegisteredClaimNames.Sub, "1")],
            notBefore: DateTime.UtcNow.AddMinutes(-10),
            expires: DateTime.UtcNow.AddMinutes(-5),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        Action validate = () => new JwtSecurityTokenHandler().ValidateToken(
            new JwtSecurityTokenHandler().WriteToken(token),
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = "test-issuer",
                ValidateAudience = true,
                ValidAudience = "test-audience",
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            },
            out _);

        validate.Should().Throw<SecurityTokenExpiredException>();
    }
}
