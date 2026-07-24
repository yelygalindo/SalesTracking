namespace SalesTracking.Infrastructure.Persistence.Settings
{
    public sealed class FrontendLinkSettings
    {
        public const string SectionName = "FrontendLinks";

        public string InvitationUrl { get; init; } = string.Empty;

        public string PasswordResetUrl { get; init; } = string.Empty;
    }
}
