using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.Units.Comands;
using SalesTracking.Application.UseCases.Units.Interfaces;
using SalesTracking.Application.UseCases.Units.Models;
using SalesTracking.Application.UseCases.Units.Results;

namespace SalesTracking.Application.UseCases.Units.Services
{
    public sealed class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;

        public UnitService(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        public async Task<UnitPagedList> GetAsync(GetUnitsCommand command)
        {
            int page = command.Page <= 0 ? 1 : command.Page;
            int pageSize = command.PageSize <= 0 ? 20 : command.PageSize;

            if (pageSize > 100)
                pageSize = 100;

            return await _unitRepository.GetAsync(
                new GetUnitsCommand(
                    string.IsNullOrWhiteSpace(command.Search) ? null : command.Search.Trim(),
                    page,
                    pageSize));
        }

        public async Task<UnitResult?> GetByExternalIdAsync(GetUnitByExternalIdCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
                return null;

            return await _unitRepository.GetByExternalIdAsync(command.ExternalId.Trim());
        }

        public async Task<CreateUnitResult> CreateAsync(CreateUnitCommand command)
        {
            CreateUnitResult? validation = Validate(command.Name, command.Symbol);
            if (validation != null)
                return validation;

            CreateUnit unit = new CreateUnit
            {
                ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.Unit),
                Name = command.Name.Trim(),
                Symbol = command.Symbol.Trim(),
                Description = command.Description?.Trim(),
                AllowsDecimals = command.AllowsDecimals,
                IsActive = command.IsActive
            };

            return await _unitRepository.CreateAsync(unit);
        }

        public async Task<UpdateUnitResult> UpdateAsync(UpdateUnitCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new UpdateUnitResult
                {
                    Succeeded = false,
                    Message = "La unidad es requerida."
                };
            }

            CreateUnitResult? validation = Validate(command.Name, command.Symbol);
            if (validation != null)
            {
                return new UpdateUnitResult
                {
                    Succeeded = false,
                    Message = validation.Message
                };
            }

            UpdateUnitCommand normalizedCommand = new UpdateUnitCommand
            {
                ExternalId = command.ExternalId.Trim(),
                Name = command.Name.Trim(),
                Symbol = command.Symbol.Trim(),
                Description = command.Description?.Trim(),
                AllowsDecimals = command.AllowsDecimals,
                IsActive = command.IsActive
            };

            return await _unitRepository.UpdateAsync(normalizedCommand);
        }

        public async Task<DeleteUnitResult> DeleteAsync(DeleteUnitCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new DeleteUnitResult
                {
                    Succeeded = false,
                    Message = "La unidad es requerida."
                };
            }

            return await _unitRepository.DeleteAsync(command.ExternalId.Trim());
        }

        private static CreateUnitResult? Validate(string name, string symbol)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new CreateUnitResult
                {
                    Succeeded = false,
                    Message = "El nombre de la unidad es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(symbol))
            {
                return new CreateUnitResult
                {
                    Succeeded = false,
                    Message = "El símbolo de la unidad es requerido."
                };
            }

            return null;
        }
    }
}