using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Host.Development;
using SalesTracking.Host.Extensions;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Logging;
using UrbanTrack.Api.Controllers;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);
if (builder.Environment.IsDevelopment())
    builder.Services.AddScoped<IPasswordResetLinkNotifier, DevelopmentPasswordResetLinkNotifier>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

JwtSettings jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings configuration is required.");

byte[] signingKey = Encoding.UTF8.GetBytes(jwtSettings.Secret);
if (signingKey.Length < 32)
    throw new InvalidOperationException("JWT Secret must be at least 32 bytes long.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(
                    new MessageResponse { Message = "Token de acceso requerido o inválido." });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(
                    new MessageResponse { Message = "No tienes permisos para realizar esta acción." });
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<SellerResourceAuthorizationFilter>();
builder.Services
    .AddControllers(options =>
    {
        options.Conventions.Add(new PermissionAuthorizationConvention());
        options.Filters.AddService<SellerResourceAuthorizationFilter>();
        AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        options.Filters.Add(new AuthorizeFilter(policy));
    })
    .AddApplicationPart(typeof(UrbanTrackApiAssemblyMarker).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfigurations();

var app = builder.Build();

InfrastructureExceptionLogger.Configure(app.Services.GetRequiredService<ILoggerFactory>());

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception exception)
    {
        app.Logger.LogError(
            exception,
            "Unhandled HTTP error for {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        if (context.Response.HasStarted)
            throw;

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(
            new MessageResponse { Message = "Ocurrió un error interno." });
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
