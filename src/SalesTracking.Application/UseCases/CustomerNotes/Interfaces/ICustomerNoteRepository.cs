using SalesTracking.Application.UseCases.CustomerNotes.Models;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.CustomerNotes.Interfaces
{
    public interface ICustomerNoteRepository
    {
        Task<IReadOnlyList<CustomerNote>> GetNotesAsync(string customerExternalId);
        Task<ResponseCreateCustomerNote> AddNoteAsync(CreateCustomerNote note);
    }
}
