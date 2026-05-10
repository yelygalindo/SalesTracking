namespace SalesTracking.Infrastructure.Persistence.Settings
{
    public class DatabaseSettings
    {
        public const string SectionName = "DatabaseSettings";
        public string ConnectionString { get; init; } = default!;
    }
}
