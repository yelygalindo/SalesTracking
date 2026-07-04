using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Interfaces;
using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Projects.Services
{
    public sealed class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<CreateProjectResult?> CreateAsync(CreateProjectCommand command)
        {
            var externalId = ExternalIdGenerator.New(ExternalIdPrefixes.Project);

            var project = Project.Create(
                externalId,
                command.Name,
                command.Description,
                command.CustomerId,
                command.SellerId,
                command.EstimatedAmount,
                command.StartDateUtc,
                command.ExpectedCloseDateUtc);

            var createdId = await _projectRepository.CreateAsync(project);

            if (createdId == null)
                return null;

            return new CreateProjectResult
            {
                Id = createdId,
                Message = "Proyecto creado correctamente."
            };
        }       

        public async Task<ProjectPagedList> GetAsync(GetProjectsCommand command)
        {
            var page = command.Page <= 0 ? 1 : command.Page;
            var pageSize = command.PageSize <= 0 ? 20 : command.PageSize;

            if (pageSize > 100)
                pageSize = 100;

            var normalizedCommand = command with
            {
                Page = page,
                PageSize = pageSize
            };

            return await _projectRepository.GetAsync(normalizedCommand);
        }

        public async Task<ProjectDetailResult?> GetByExternalIdAsync(GetProjectByExternalIdCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
                return null;

            var normalizedCommand = command with
            {
                ExternalId = command.ExternalId.Trim()
            };

            return await _projectRepository.GetByExternalIdAsync(normalizedCommand);
        }

        public async Task<ChangeProjectStatusResult> ChangeStatusAsync(ChangeProjectStatusCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new ChangeProjectStatusResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            if (command.StatusId <= 0)
            {
                return new ChangeProjectStatusResult
                {
                    Succeeded = false,
                    Message = "Estado de proyecto inválido."
                };
            }

            var normalizedCommand = new ChangeProjectStatusCommand
            {
                ExternalId = command.ExternalId.Trim(),
                StatusId = command.StatusId
            };

            return await _projectRepository.ChangeStatusAsync(normalizedCommand);
        }
    }
}
