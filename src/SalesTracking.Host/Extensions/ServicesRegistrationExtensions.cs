using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Services;
using SalesTracking.Application.UseCases.CustomerNotes.Interfaces;
using SalesTracking.Application.UseCases.CustomerNotes.Services;
using SalesTracking.Application.UseCases.CustomerReminders.Interfaces;
using SalesTracking.Application.UseCases.CustomerReminders.Services;
using SalesTracking.Application.UseCases.Customers.Interfaces;
using SalesTracking.Application.UseCases.Customers.Services;
using SalesTracking.Application.UseCases.Invitations.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Services;
using SalesTracking.Application.UseCases.ProjectNotes.Interfaces;
using SalesTracking.Application.UseCases.ProjectNotes.Services;
using SalesTracking.Application.UseCases.Projects.Interfaces;
using SalesTracking.Application.UseCases.Projects.Services;
using SalesTracking.Application.UseCases.ProjectTimeline.Interfaces;
using SalesTracking.Application.UseCases.ProjectTimeline.Services;
using SalesTracking.Application.UseCases.Products.Interfaces;
using SalesTracking.Application.UseCases.Products.Services;
using SalesTracking.Infrastructure.Persistence.Security;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Auth;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders;
using SalesTracking.Infrastructure.Persistence.Sql.Customers;
using SalesTracking.Infrastructure.Persistence.Sql.Invitations;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes;
using SalesTracking.Infrastructure.Persistence.Sql.Projects;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline;
using SalesTracking.Infrastructure.Persistence.Sql.Products;

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
            services.AddScoped<IProjectService, ProjectService>();
            
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerNoteService, CustomerNoteService>();
            services.AddScoped<ICustomerReminderService, CustomerReminderService>();
            services.AddScoped<IProjectNoteService, ProjectNoteService>();
            services.AddScoped<IProjectTimelineService, ProjectTimelineService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectNoteRepository, ProjectNoteRepository>();
            services.AddScoped<IProjectTimelineRepository, ProjectTimelineRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerNoteRepository, CustomerNoteRepository>();
            services.AddScoped<ICustomerReminderRepository, CustomerReminderRepository>();

            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            return services;
        }
    }
}