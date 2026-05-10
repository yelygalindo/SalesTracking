namespace SalesTracking.Application.UseCases.Authentication.Comands
{
    public class LoginCommand
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}
