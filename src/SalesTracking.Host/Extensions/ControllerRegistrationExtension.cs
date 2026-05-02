using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using UrbanTrack.Api.Configurations;

namespace SalesTracking.Host.Extensions
{
    public static class ControllerRegistrationExtension
    {
        public static IServiceCollection AddControllersWithFilters(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            string[] schemes;
            IEnumerable<JwtAuthOptions> jwtOptions = configuration.GetSection("JwtAuth").Get<IEnumerable<JwtAuthOptions>>()?.Where(o => o.Enabled);
            bool existsEnabledAuth = jwtOptions != null && jwtOptions.Any();

            if (environment.IsProduction() || existsEnabledAuth)
            {
                schemes = jwtOptions.Select(o => o.SchemeName).ToArray();
            }
            else
            {
                schemes = ["Local"];
            }

            AuthorizationPolicy multiSchemePolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(schemes)
                .RequireAuthenticatedUser()
                .Build();

            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter(multiSchemePolicy));
                //options.Filters.Add<IdentityFilter>();
            });

            return services;
        }
    }
}
