using SalesTracking.Application.Common.Models;

namespace SalesTracking.Application.Common.Interfaces
{
    public interface IFileStorage
    {
        Task SaveAsync(string storageKey, Stream content);
        Task<StoredFile?> OpenReadAsync(string storageKey);
        Task DeleteAsync(string storageKey);
    }
}
