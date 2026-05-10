using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Services;
using SalesTracking.Infrastructure.Persistence.Security;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Auth;

namespace SalesTracking.Host.Extensions
{
    public static class ServicesRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName));
            services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
            
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            return services;
        }
    }
}
