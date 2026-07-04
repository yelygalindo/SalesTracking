using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using SalesTracking.Application.UseCases.ProjectNotes.Results;

namespace SalesTracking.Application.UseCases.ProjectNotes.Interfaces
{
    public interface IProjectNoteService
    {
        Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(GetProjectNotesCommand command);
    }
}
