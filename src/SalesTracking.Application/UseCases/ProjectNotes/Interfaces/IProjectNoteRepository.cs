using SalesTracking.Application.UseCases.ProjectNotes.Models;
using SalesTracking.Application.UseCases.ProjectNotes.Results;

namespace SalesTracking.Application.UseCases.ProjectNotes.Interfaces
{
    public interface IProjectNoteRepository
    {
        Task<ResponseCreateProjectNote> AddNoteAsync(CreateProjectNote note);
        Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(string projectExternalId);
    }
}
