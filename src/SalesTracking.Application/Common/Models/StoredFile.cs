namespace SalesTracking.Application.Common.Models
{
    public sealed class StoredFile
    {
        public string StorageKey { get; set; } = string.Empty;
        public Stream Content { get; set; } = Stream.Null;
    }
}
