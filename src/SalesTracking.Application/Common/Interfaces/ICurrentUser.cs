namespace SalesTracking.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }
        int? UserId { get; }
        string? UserExternalId { get; }
        int? CompanyId { get; }
        string? Email { get; }
        string? Name { get; }
        IReadOnlyCollection<string> Roles { get; }
    }
}