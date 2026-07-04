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
            if (command == null)
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "La solicitud no es válida."
                };
            }

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "El nombre del proyecto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.CustomerId))
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "El cliente es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.SellerId))
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "El vendedor es requerido."
                };
            }

            if (command.Latitude is < -90 or > 90)
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "La latitud no es válida."
                };
            }

            if (command.Longitude is < -180 or > 180)
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "La longitud no es válida."
                };
            }

            var externalId = ExternalIdGenerator.New(ExternalIdPrefixes.Project);

            var project = Project.Create(
                externalId,
                command.Name,
                command.Description,
                command.CustomerId,
                command.SellerId,
                command.EstimatedAmount,
                command.StartDateUtc,
                command.ExpectedCloseDateUtc,
                command.Address,
                command.Latitude,
                command.Longitude);

            return await _projectRepository.CreateAsync(project);
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

        public async Task<UpdateProjectResult> UpdateAsync(UpdateProjectCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "El nombre del proyecto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.CustomerExternalId))
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "El cliente es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(command.SellerExternalId))
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "El vendedor es requerido."
                };
            }

            if (command.Latitude is < -90 or > 90)
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "La latitud no es válida."
                };
            }

            if (command.Longitude is < -180 or > 180)
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "La longitud no es válida."
                };
            }

            var normalizedCommand = new UpdateProjectCommand
            {
                ExternalId = command.ExternalId.Trim(),
                Name = command.Name.Trim(),
                Description = command.Description?.Trim(),
                CustomerExternalId = command.CustomerExternalId.Trim(),
                SellerExternalId = command.SellerExternalId.Trim(),
                EstimatedAmount = command.EstimatedAmount,
                StartDateUtc = command.StartDateUtc,
                ExpectedCloseDateUtc = command.ExpectedCloseDateUtc,
                Address = command.Address?.Trim(),
                Latitude = command.Latitude,
                Longitude = command.Longitude
            };

            return await _projectRepository.UpdateAsync(normalizedCommand);
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

        public async Task<DeleteProjectResult> DeleteAsync(DeleteProjectCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new DeleteProjectResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            var normalizedCommand = command with
            {
                ExternalId = command.ExternalId.Trim()
            };

            return await _projectRepository.DeleteAsync(normalizedCommand);
        }
    }
}
