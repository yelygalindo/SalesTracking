namespace SalesTracking.Infrastructure.Persistence.Settings
{
    public sealed class StorageSettings
    {
        public const string SectionName = "Storage";
        public string RootPath { get; set; } = "storage";
    }
}
