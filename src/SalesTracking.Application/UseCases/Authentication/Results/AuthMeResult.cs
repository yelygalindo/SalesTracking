namespace SalesTracking.Application.UseCases.Authentication.Results;

public sealed class AuthMeResult
{
    public int Id { get; init; }
    public string ExternalId { get; init; } = default!;
    public string Username { get; init; } = default!;
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public int CompanyId { get; init; }
    public string CompanyExternalId { get; init; } = default!;
    public string CompanyName { get; init; } = default!;
    public IReadOnlyCollection<string> Roles { get; init; } = [];
    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}
