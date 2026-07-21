using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.Common.Models;
using SalesTracking.Infrastructure.Persistence.Settings;

namespace SalesTracking.Infrastructure.Storage
{
    public sealed class LocalFileStorage : IFileStorage
    {
        private readonly StorageSettings _storageSettings;

        public LocalFileStorage(IOptions<StorageSettings> storageSettings)
        {
            _storageSettings = storageSettings.Value
                ?? throw new ArgumentNullException(nameof(storageSettings));
        }

        public async Task SaveAsync(string storageKey, Stream content)
        {
            string path = GetSafePath(storageKey);
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            if (content.CanSeek)
                content.Position = 0;

            await using FileStream fileStream = new FileStream(
                path,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await content.CopyToAsync(fileStream);
        }

        public Task<StoredFile?> OpenReadAsync(string storageKey)
        {
            string path = GetSafePath(storageKey);
            if (!File.Exists(path))
                return Task.FromResult<StoredFile?>(null);

            Stream stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);

            return Task.FromResult<StoredFile?>(new StoredFile
            {
                StorageKey = storageKey,
                Content = stream
            });
        }

        public Task DeleteAsync(string storageKey)
        {
            string path = GetSafePath(storageKey);
            if (File.Exists(path))
                File.Delete(path);

            return Task.CompletedTask;
        }

        private string GetSafePath(string storageKey)
        {
            string rootPath = Path.GetFullPath(_storageSettings.RootPath);
            string relativePath = storageKey.Replace('/', Path.DirectorySeparatorChar);
            string fullPath = Path.GetFullPath(Path.Combine(rootPath, relativePath));

            if (!fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("StorageKey no valido.");

            return fullPath;
        }
    }
}
