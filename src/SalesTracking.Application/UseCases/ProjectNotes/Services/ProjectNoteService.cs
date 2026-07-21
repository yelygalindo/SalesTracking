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
                    Message = "La solicitud no es valida."
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

            if (command.AuthorUserId <= 0)
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
                AuthorUserId = command.AuthorUserId
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

        public async Task<UpdateProjectNoteResult> UpdateNoteAsync(UpdateProjectNoteCommand command)
        {
            if (command == null)
            {
                return new UpdateProjectNoteResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es valida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.ProjectExternalId))
            {
                return new UpdateProjectNoteResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.NoteExternalId))
            {
                return new UpdateProjectNoteResult
                {
                    Succeeded = false,
                    Message = "La nota es requerida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.Content))
            {
                return new UpdateProjectNoteResult
                {
                    Succeeded = false,
                    Message = "El contenido de la nota es requerido."
                };
            }

            if (command.UpdatedByUserId <= 0)
            {
                return new UpdateProjectNoteResult
                {
                    Succeeded = false,
                    Message = "El usuario que actualiza es requerido."
                };
            }

            UpdateProjectNote note = new UpdateProjectNote
            {
                ProjectExternalId = command.ProjectExternalId.Trim(),
                NoteExternalId = command.NoteExternalId.Trim(),
                Content = command.Content.Trim(),
                UpdatedByUserId = command.UpdatedByUserId
            };

            ResponseUpdateProjectNote updated = await _projectNoteRepository.UpdateNoteAsync(note);

            if (!updated.Succeeded)
            {
                return new UpdateProjectNoteResult
                {
                    Succeeded = false,
                    NotFound = updated.NotFound,
                    Message = updated.Message ?? "No se pudo actualizar la nota."
                };
            }

            return new UpdateProjectNoteResult
            {
                Succeeded = true,
                Message = "Nota actualizada correctamente."
            };
        }

        public async Task<DeleteProjectNoteResult> DeleteNoteAsync(DeleteProjectNoteCommand command)
        {
            if (command == null)
            {
                return new DeleteProjectNoteResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es valida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.ProjectExternalId))
            {
                return new DeleteProjectNoteResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.NoteExternalId))
            {
                return new DeleteProjectNoteResult
                {
                    Succeeded = false,
                    Message = "La nota es requerida."
                };
            }

            ResponseDeleteProjectNote deleted = await _projectNoteRepository.DeleteNoteAsync(
                command with
                {
                    ProjectExternalId = command.ProjectExternalId.Trim(),
                    NoteExternalId = command.NoteExternalId.Trim()
                });

            if (!deleted.Succeeded)
            {
                return new DeleteProjectNoteResult
                {
                    Succeeded = false,
                    NotFound = deleted.NotFound,
                    Message = deleted.Message ?? "No se pudo eliminar la nota."
                };
            }

            return new DeleteProjectNoteResult
            {
                Succeeded = true,
                Message = "Nota eliminada correctamente."
            };
        }

        public async Task<ProjectNoteResult?> GetNoteAsync(GetProjectNoteCommand command)
        {
            if (command == null ||
                string.IsNullOrWhiteSpace(command.ProjectExternalId) ||
                string.IsNullOrWhiteSpace(command.NoteExternalId))
                return null;

            return await _projectNoteRepository.GetNoteAsync(
                command.ProjectExternalId.Trim(),
                command.NoteExternalId.Trim());
        }

        public async Task<IReadOnlyList<ProjectNoteResult>> GetNotesAsync(GetProjectNotesCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ProjectExternalId))
                return [];

            return await _projectNoteRepository.GetNotesAsync(command.ProjectExternalId.Trim());
        }
    }
}
