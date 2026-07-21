using SalesTracking.Application.UseCases.ProjectNotes.Models;
using SalesTracking.Application.UseCases.ProjectNotes.Results;

namespace SalesTracking.Application.UseCases.ProjectNotes.Interfaces
{
    public interface IProjectNoteRepository
    {
        Task<ResponseCreateProjectNote> AddNoteAsync(CreateProjectNote note);
        Task<ResponseUpdateProjectNote> UpdateNoteAsync(UpdateProjectNote note);
        Task<ResponseDeleteProjectNote> DeleteNoteAsync(string projectExternalId, string noteExternalId);
        Task<ProjectNoteResult?> GetNoteAsync(string projectExternalId, string noteExternalId);
        Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(string projectExternalId);
    }
}