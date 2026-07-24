namespace SalesTracking.Infrastructure.Persistence.Settings
{
    public sealed class MicrosoftGraphSettings
    {
        public const string SectionName = "MicrosoftGraph";

        public string TenantId { get; init; } = string.Empty;
        public string ClientId { get; init; } = string.Empty;
        public string ClientSecret { get; init; } = string.Empty;
        public string SenderEmail { get; init; } = string.Empty;
    }
}
