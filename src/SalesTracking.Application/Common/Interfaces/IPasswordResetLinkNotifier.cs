using SalesTracking.Application.UseCases.Authentication.Models;

namespace SalesTracking.Application.Common.Interfaces
{
    public interface IPasswordResetLinkNotifier
    {
        Task NotifyAsync(PasswordForgot passwordForgot);
    }
}
