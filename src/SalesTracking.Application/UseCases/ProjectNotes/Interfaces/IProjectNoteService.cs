using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using SalesTracking.Application.UseCases.ProjectNotes.Results;

namespace SalesTracking.Application.UseCases.ProjectNotes.Interfaces
{
    public interface IProjectNoteService
    {
        Task<AddProjectNoteResult> AddNoteAsync(AddProjectNoteCommand command);
        Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(GetProjectNotesCommand command);
    }
}
