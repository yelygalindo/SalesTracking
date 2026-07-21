using Microsoft.AspNetCore.Authorization;

namespace UrbanTrack.Api.Security;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        bool isSeller = context.User.FindAll(System.Security.Claims.ClaimTypes.Role)
            .Any(claim => string.Equals(claim.Value, "seller", StringComparison.OrdinalIgnoreCase));
        bool catalogAdministration = requirement.Permission is
            "products.create" or "products.update" or "products.delete" or
            "units.create" or "units.update" or "units.delete";
        if (isSeller && (requirement.Permission == "invitations.create" || catalogAdministration))
            return Task.CompletedTask;

        if (context.User.FindAll("permission").Any(claim =>
                string.Equals(claim.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase)))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
