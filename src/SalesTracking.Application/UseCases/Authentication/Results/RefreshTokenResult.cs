namespace SalesTracking.Application.UseCases.Authentication.Results
{
    public class RefreshTokenResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}
