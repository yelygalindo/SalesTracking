using SalesTracking.Application.UseCases.Authentication.Models;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Authentication.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<AuthTokens?> RefreshTokensAsync(string lastToken);
        Task<PasswordForgot?> SendForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<Invitation?> GetInvitationByTokenAsync(string token);
        Task<AcceptInvitation> AcceptInvitationAsync(AcceptInvitationInput request);
        Task<AuthTokens?> ValidateCredentialsAsync(string username, string password);
        Task<Invitation?> CreateInvitationAsync(CreateInvitation createInvitation);
    }
}
