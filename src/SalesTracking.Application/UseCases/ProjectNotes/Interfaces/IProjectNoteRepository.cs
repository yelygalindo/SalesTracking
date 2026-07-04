using SalesTracking.Application.UseCases.ProjectNotes.Results;

namespace SalesTracking.Application.UseCases.ProjectNotes.Interfaces
{
    public interface IProjectNoteRepository
    {
        Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(string projectExternalId);
    }
}
