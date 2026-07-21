using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using SalesTracking.Application.Common.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;

namespace UrbanTrack.Api.Security;

public sealed class PermissionAuthorizationConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (ControllerModel controller in application.Controllers)
        foreach (ActionModel action in controller.Actions)
        {
            if (action.Attributes.OfType<IAllowAnonymous>().Any())
                continue;

            string? operation = ResolveOperation(action);
            string? policy = ResolvePermission(controller.ControllerName, action.ActionName, operation);
            if (policy == null)
                continue;

            action.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
        }
    }

    private static string? ResolvePermission(string controller, string action, string? operation)
    {
        if (controller == "Auth" && action.Contains("Logout", StringComparison.OrdinalIgnoreCase)) return Permissions.AuthLogout;
        if (controller == "Invitations" && operation == "create") return Permissions.InvitationsCreate;
        if (controller is "Dashboard") return Permissions.DashboardRead;
        if (controller is "Reports") return Permissions.ReportsRead;
        if (controller is "Sellers") return Permissions.UsersRead;

        string? resource = controller switch
        {
            "Customers" or "CustomerNotes" or "CustomerReminders" or "CustomerTimeline" => "customers",
            "Projects" or "ProjectNotes" or "ProjectAttachments" or "ProjectTimeline" or "ProjectVisits" or "ProjectMaterials" => "projects",
            "Deliveries" or "DeliveryStatuses" => "deliveries",
            "Products" => "products",
            "Units" => "units",
            _ => null
        };
        return (resource, operation) switch
        {
            ("customers", "read") => Permissions.CustomersRead,
            ("customers", "create") => Permissions.CustomersCreate,
            ("customers", "update") => Permissions.CustomersUpdate,
            ("customers", "delete") => Permissions.CustomersDelete,
            ("projects", "read") => Permissions.ProjectsRead,
            ("projects", "create") => Permissions.ProjectsCreate,
            ("projects", "update") => Permissions.ProjectsUpdate,
            ("projects", "delete") => Permissions.ProjectsDelete,
            ("deliveries", "read") => Permissions.DeliveriesRead,
            ("deliveries", "create") => Permissions.DeliveriesCreate,
            ("deliveries", "update") => Permissions.DeliveriesUpdate,
            ("deliveries", "delete") => Permissions.DeliveriesDelete,
            ("products", "read") => Permissions.ProductsRead,
            ("products", "create") => Permissions.ProductsCreate,
            ("products", "update") => Permissions.ProductsUpdate,
            ("products", "delete") => Permissions.ProductsDelete,
            ("units", "read") => Permissions.UnitsRead,
            ("units", "create") => Permissions.UnitsCreate,
            ("units", "update") => Permissions.UnitsUpdate,
            ("units", "delete") => Permissions.UnitsDelete,
            _ => null
        };
    }

    private static string? ResolveOperation(ActionModel action)
    {
        string[] methods = action.Attributes
            .OfType<IActionHttpMethodProvider>()
            .SelectMany(provider => provider.HttpMethods ?? [])
            .ToArray();

        if (methods.Contains("GET", StringComparer.OrdinalIgnoreCase)) return "read";
        if (methods.Contains("DELETE", StringComparer.OrdinalIgnoreCase)) return "delete";
        if (methods.Contains("PUT", StringComparer.OrdinalIgnoreCase) || methods.Contains("PATCH", StringComparer.OrdinalIgnoreCase)) return "update";
        if (methods.Contains("POST", StringComparer.OrdinalIgnoreCase)) return "create";
        return null;
    }
}
