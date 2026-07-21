using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.ProjectNotes.Comands;
using SalesTracking.Application.UseCases.ProjectNotes.Interfaces;
using SalesTracking.Application.UseCases.ProjectNotes.Models;
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

        public async Task<AddProjectNoteResult> AddNoteAsync(AddProjectNoteCommand command)
        {
            if (command == null)
            {
                return new AddProjectNoteResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es válida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.ProjectExternalId))
            {
                return new AddProjectNoteResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.Content))
            {
                return new AddProjectNoteResult
                {
                    Succeeded = false,
                    Message = "La nota es requerida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.AuthorExternalId))
            {
                return new AddProjectNoteResult
                {
                    Succeeded = false,
                    Message = "El autor es requerido."
                };
            }

            CreateProjectNote note = new CreateProjectNote
            {
                ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.ProjectNote),
                ProjectExternalId = command.ProjectExternalId.Trim(),
                Content = command.Content.Trim(),
                AuthorExternalId = command.AuthorExternalId.Trim()
            };

            ResponseCreateProjectNote created = await _projectNoteRepository.AddNoteAsync(note);

            if (!created.Succeeded)
            {
                return new AddProjectNoteResult
                {
                    Succeeded = false,
                    NotFound = created.NotFound,
                    Message = created.Message ?? "No se pudo agregar la nota."
                };
            }

            return new AddProjectNoteResult
            {
                Succeeded = true,
                Id = created.CreateProjectNote.ExternalId,
                Message = "Nota agregada correctamente."
            };
        }

        public async Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(GetProjectNotesCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ProjectExternalId))
                return [];

            return await _projectNoteRepository.GetNotesAsync(command.ProjectExternalId.Trim());
        }
    }
}
