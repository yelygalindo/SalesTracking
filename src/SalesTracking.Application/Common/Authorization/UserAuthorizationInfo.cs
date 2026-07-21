namespace SalesTracking.Application.Common.Authorization;

public sealed record UserAuthorizationInfo(
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
