using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Projects.Comands;
using SalesTracking.Application.UseCases.Projects.Interfaces;
using SalesTracking.Application.UseCases.Projects.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Projects.Services
{
    public sealed class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUser _currentUser;

        public ProjectService(IProjectRepository projectRepository, ICurrentUser currentUser)
        {
            _projectRepository = projectRepository;
            _currentUser = currentUser;
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

            string sellerExternalId = IsSeller
                ? _currentUser.UserExternalId
                : Normalize(command.SellerId) ?? _currentUser.UserExternalId;

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

            if (command.ProgressPercentage is < 0 or > 100)
            {
                return new CreateProjectResult
                {
                    Succeeded = false,
                    Message = "El porcentaje de avance no es válido."
                };
            }

            var externalId = ExternalIdGenerator.New(ExternalIdPrefixes.Project);

            var project = Project.Create(
                externalId,
                command.Name,
                command.Description,
                command.CustomerId,
                sellerExternalId,
                command.EstimatedAmount,
                command.StartDateUtc,
                command.ExpectedCloseDateUtc,
                command.ProgressPercentage ?? 0,
                command.ActualCloseDateUtc,
                command.Address,
                command.Latitude,
                command.Longitude);

            return await _projectRepository.CreateAsync(project, command.CreatedByUserId);
        }       

        public async Task<ProjectPagedList> GetAsync(GetProjectsCommand command)
        {
            var page = command.Page <= 0 ? 1 : command.Page;
            var pageSize = command.PageSize <= 0 ? 20 : command.PageSize;

            if (pageSize > 100)
                pageSize = 100;

            var normalizedCommand = command with
            {
                Status = Normalize(command.Status),
                CustomerId = Normalize(command.CustomerId),
                SellerId = Normalize(command.SellerId),
                Page = page,
                PageSize = pageSize
            };

            return await _projectRepository.GetAsync(normalizedCommand);
        }

        private static string? Normalize(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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

            if (command.ProgressPercentage is < 0 or > 100)
            {
                return new UpdateProjectResult
                {
                    Succeeded = false,
                    Message = "El porcentaje de avance no es válido."
                };
            }

            var normalizedCommand = new UpdateProjectCommand
            {
                ExternalId = command.ExternalId.Trim(),
                Name = command.Name.Trim(),
                Description = command.Description?.Trim(),
                CustomerExternalId = command.CustomerExternalId.Trim(),
                SellerExternalId = IsSeller
                    ? _currentUser.UserExternalId
                    : Normalize(command.SellerExternalId),
                EstimatedAmount = command.EstimatedAmount,
                StartDateUtc = command.StartDateUtc,
                ExpectedCloseDateUtc = command.ExpectedCloseDateUtc,
                ProgressPercentage = command.ProgressPercentage ?? 0,
                ActualCloseDateUtc = command.ActualCloseDateUtc,
                Address = command.Address?.Trim(),
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                UpdatedByUserId = command.UpdatedByUserId
            };

            return await _projectRepository.UpdateAsync(normalizedCommand);
        }

        private bool IsSeller =>
            _currentUser.Roles.Contains("seller", StringComparer.OrdinalIgnoreCase);

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
                StatusId = command.StatusId,
                ChangedByUserId = command.ChangedByUserId
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
