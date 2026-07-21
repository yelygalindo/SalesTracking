using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Interfaces;
using SalesTracking.Application.UseCases.Authentication.Services;
using SalesTracking.Application.UseCases.CustomerNotes.Interfaces;
using SalesTracking.Application.UseCases.CustomerNotes.Services;
using SalesTracking.Application.UseCases.CustomerReminders.Interfaces;
using SalesTracking.Application.UseCases.CustomerReminders.Services;
using SalesTracking.Application.UseCases.Customers.Interfaces;
using SalesTracking.Application.UseCases.Customers.Services;
using SalesTracking.Application.UseCases.CustomerTimeline.Interfaces;
using SalesTracking.Application.UseCases.CustomerTimeline.Services;
using SalesTracking.Application.UseCases.Deliveries.Interfaces;
using SalesTracking.Application.UseCases.Deliveries.Services;
using SalesTracking.Application.UseCases.Dashboard.Interfaces;
using SalesTracking.Application.UseCases.Dashboard.Services;
using SalesTracking.Application.UseCases.Invitations.Interfaces;
using SalesTracking.Application.UseCases.Invitations.Services;
using SalesTracking.Application.UseCases.ProjectAttachments.Interfaces;
using SalesTracking.Application.UseCases.ProjectAttachments.Services;
using SalesTracking.Application.UseCases.ProjectNotes.Interfaces;
using SalesTracking.Application.UseCases.ProjectNotes.Services;
using SalesTracking.Application.UseCases.Projects.Interfaces;
using SalesTracking.Application.UseCases.Projects.Services;
using SalesTracking.Application.UseCases.ProjectTimeline.Interfaces;
using SalesTracking.Application.UseCases.ProjectTimeline.Services;
using SalesTracking.Application.UseCases.ProjectVisits.Interfaces;
using SalesTracking.Application.UseCases.ProjectVisits.Services;
using SalesTracking.Application.UseCases.Products.Interfaces;
using SalesTracking.Application.UseCases.Products.Services;
using SalesTracking.Application.UseCases.Reports.Interfaces;
using SalesTracking.Application.UseCases.Reports.Services;
using SalesTracking.Application.UseCases.Units.Interfaces;
using SalesTracking.Application.UseCases.Units.Services;
using SalesTracking.Infrastructure.Persistence.Security;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Auth;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders;
using SalesTracking.Infrastructure.Persistence.Sql.Customers;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerTimeline;
using SalesTracking.Infrastructure.Persistence.Sql.Dashboard;
using SalesTracking.Infrastructure.Persistence.Sql.Deliveries;
using SalesTracking.Infrastructure.Persistence.Sql.Invitations;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectAttachments;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectNotes;
using SalesTracking.Infrastructure.Persistence.Sql.Projects;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectVisits;
using SalesTracking.Infrastructure.Persistence.Sql.Products;
using SalesTracking.Infrastructure.Persistence.Sql.Reports;
using SalesTracking.Infrastructure.Persistence.Sql.Units;
using SalesTracking.Infrastructure.Storage;

namespace SalesTracking.Host.Extensions
{
    public static class ServicesRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName));
            services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
            services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.SectionName));
            
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProjectService, ProjectService>();
            
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerNoteService, CustomerNoteService>();
            services.AddScoped<ICustomerReminderService, CustomerReminderService>();
            services.AddScoped<ICustomerTimelineService, CustomerTimelineService>();
            services.AddScoped<IProjectNoteService, ProjectNoteService>();
            services.AddScoped<IProjectTimelineService, ProjectTimelineService>();
            services.AddScoped<IProjectAttachmentService, ProjectAttachmentService>();
            services.AddScoped<IProjectVisitService, ProjectVisitService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IReportService, ReportService>();

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectNoteRepository, ProjectNoteRepository>();
            services.AddScoped<IProjectTimelineRepository, ProjectTimelineRepository>();
            services.AddScoped<IProjectAttachmentRepository, ProjectAttachmentRepository>();
            services.AddScoped<IProjectVisitRepository, ProjectVisitRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerNoteRepository, CustomerNoteRepository>();
            services.AddScoped<ICustomerReminderRepository, CustomerReminderRepository>();
            services.AddScoped<ICustomerTimelineRepository, CustomerTimelineRepository>();

            services.AddScoped<IFileStorage, LocalFileStorage>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            return services;
        }
    }
}
