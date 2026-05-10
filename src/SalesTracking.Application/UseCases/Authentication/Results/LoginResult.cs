namespace SalesTracking.Application.UseCases.Authentication.Results
{
    public class LoginResult
    {
        public UserResult User { get; set; }
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}