using Microsoft.AspNetCore.Authorization;

namespace UrbanTrack.Api.Security;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;
