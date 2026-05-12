using SalesTracking.Application.UseCases.CustomerNotes.Comands;
using SalesTracking.Application.UseCases.CustomerNotes.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.CustomerNotes.Interfaces
{
    public interface ICustomerNoteService
    {
        Task<IReadOnlyList<CustomerNote>> GetNotesAsync(GetCustomerNotesCommand command);
        Task<AddCustomerNoteResult> AddNoteAsync(AddCustomerNoteCommand command);
    }
}