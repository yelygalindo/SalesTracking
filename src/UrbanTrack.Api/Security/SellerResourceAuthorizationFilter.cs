using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SalesTracking.Application.Common.Interfaces;
using Microsoft.AspNetCore.Routing;

namespace UrbanTrack.Api.Security;

public sealed class SellerResourceAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly ICurrentUser _currentUser;
    private readonly IResourceOwnershipRepository _ownership;

    public SellerResourceAuthorizationFilter(ICurrentUser currentUser, IResourceOwnershipRepository ownership)
    {
        _currentUser = currentUser;
        _ownership = ownership;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!_currentUser.Roles.Contains("seller", StringComparer.OrdinalIgnoreCase)) return;

        string controller = context.RouteData.Values["controller"]?.ToString() ?? string.Empty;
        (string Resource, string? Id) target = ResolveTarget(controller, context.RouteData.Values);
        if (target.Id == null) return;

        if (!await _ownership.IsAssignedToSellerAsync(target.Resource, target.Id, _currentUser.CompanyId, _currentUser.UserId))
            context.Result = new NotFoundResult();
    }

    private static (string, string?) ResolveTarget(string controller, RouteValueDictionary route) => controller switch
    {
        "Customers" => ("customer", Value(route, "externalId")),
        "CustomerNotes" or "CustomerReminders" or "CustomerTimeline" => ("customer", Value(route, "customerExternalId")),
        "Projects" => ("project", Value(route, "externalId")),
        "ProjectNotes" or "ProjectAttachments" or "ProjectTimeline" or "ProjectVisits" or "ProjectMaterials" => ("project", Value(route, "projectExternalId")),
        "Deliveries" => ("delivery", Value(route, "deliveryExternalId")),
        _ => (string.Empty, null)
    };

    private static string? Value(RouteValueDictionary route, string key) => route.TryGetValue(key, out object? value) ? value?.ToString() : null;
}
