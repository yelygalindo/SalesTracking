using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using SalesTracking.Application.UseCases.ProjectNotes.Results;

namespace SalesTracking.Application.UseCases.ProjectNotes.Interfaces
{
    public interface IProjectNoteService
    {
        Task<AddProjectNoteResult> AddNoteAsync(AddProjectNoteCommand command);
        Task<UpdateProjectNoteResult> UpdateNoteAsync(UpdateProjectNoteCommand command);
        Task<DeleteProjectNoteResult> DeleteNoteAsync(DeleteProjectNoteCommand command);
        Task<ProjectNoteResult?> GetNoteAsync(GetProjectNoteCommand command);
        Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(GetProjectNotesCommand command);
    }
}