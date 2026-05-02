using Microsoft.OpenApi.Models;

namespace SalesTracking.Host.Extensions
{
    public static class ServicesRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();         

            return services;
        }
    }

    //public static IApplicationBuilder UseSwaggerConfigurations(this IApplicationBuilder app, IWebHostEnvironment env)
    //    {

    //        app.UseSwagger(c =>
    //        {
    //            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    //            {
    //                string baseUrl = httpReq.Host.Host.Contains("localhost") ? "/" : "/voltron";

    //                swaggerDoc.Servers = new List<OpenApiServer>
    //            {
    //                new OpenApiServer
    //                {
    //                    Url = baseUrl
    //                }
    //            };
    //            });
    //        });

    //        app.UseSwaggerUI(c =>
    //        {
    //            c.SwaggerEndpoint("./swagger/v1/swagger.json", "Voltron API V1");
    //            c.RoutePrefix = string.Empty;
    //        });

    //        return app;
    //    }
    //}
}
