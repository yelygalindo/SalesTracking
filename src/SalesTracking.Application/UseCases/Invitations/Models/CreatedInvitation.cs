namespace SalesTracking.Application.UseCases.Invitations.Models;

public sealed class CreatedInvitation
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}
