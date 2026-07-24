namespace SalesTracking.Infrastructure.Persistence.Settings
{
    public class AuthSettings
    {
        public const string SectionName = "AuthSettings";

        public int AccessTokenExpirationHours { get; init; }

        public int RefreshTokenExpirationDays { get; init; }

        public int PasswordResetTokenExpirationHours { get; init; }

        public string PasswordResetUrl { get; init; } = string.Empty;
    }
}
