using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using SalesTracking.Application.UseCases.ProjectNotes.Interfaces;
using SalesTracking.Application.UseCases.ProjectNotes.Results;

namespace SalesTracking.Application.UseCases.ProjectNotes.Services
{
    public sealed class ProjectNoteService : IProjectNoteService
    {
        private readonly IProjectNoteRepository _projectNoteRepository;

        public ProjectNoteService(IProjectNoteRepository projectNoteRepository)
        {
            _projectNoteRepository = projectNoteRepository;
        }

        public async Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(GetProjectNotesCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ProjectExternalId))
                return [];

            return await _projectNoteRepository.GetNotesAsync(command.ProjectExternalId.Trim());
        }
    }
}
