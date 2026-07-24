namespace SalesTracking.Application.UseCases.Authentication.Comands
{
    public class LogoutComand
    {
        public string RefreshToken { get; set; }
        public string? DeviceId { get; set; }
    }
}
