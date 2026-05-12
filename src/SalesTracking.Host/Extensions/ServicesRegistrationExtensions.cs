using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Services;
using SalesTracking.Application.UseCases.CustomerNotes.Interfaces;
using SalesTracking.Application.UseCases.CustomerNotes.Services;
using SalesTracking.Application.UseCases.Customers.Interfaces;
using SalesTracking.Application.UseCases.Customers.Services;
using SalesTracking.Application.UseCases.Invitations.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Services;
using SalesTracking.Infrastructure.Persistence.Security;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Auth;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes;
using SalesTracking.Infrastructure.Persistence.Sql.Customers;
using SalesTracking.Infrastructure.Persistence.Sql.Invitations;

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
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerNoteService, CustomerNoteService>();

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerNoteRepository, CustomerNoteRepository>();
           
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            return services;
        }
    }
}
