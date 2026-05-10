namespace SalesTracking.Application.UseCases.Authentication.Comands
{
    public class ResetPasswordComand
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
