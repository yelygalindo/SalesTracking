using SalesTracking.Application.UseCases.Authentication.Comands;
using SalesTracking.Application.UseCases.Authentication.Results;

namespace SalesTracking.Application.UseCases.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginCommand loginCommand);
        Task<LogoutResult> LogoutAsync(LogoutComand logoutComand);
        Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenComand refreshTokenComand);
        Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordComand forgotPasswordComand);
        Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordComand resetPasswordComand);
        Task<InvitationResult?> GetInvitationByTokenAsync(GetInvitationByTokenComand getInvitationByTokenComand);
        Task<AcceptInvitationResult> AcceptInvitationAsync(AcceptInvitationComand acceptInvitationComand);
        Task<CreateInvitationResult> CreateInvitationAsync(CreateInvitationComand createInvitationComand);
    }
}