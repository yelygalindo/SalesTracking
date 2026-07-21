using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.ProjectVisits.Comands;
using SalesTracking.Application.UseCases.ProjectVisits.Interfaces;
using SalesTracking.Application.UseCases.ProjectVisits.Models;
using SalesTracking.Application.UseCases.ProjectVisits.Results;

namespace SalesTracking.Application.UseCases.ProjectVisits.Services;

public sealed class ProjectVisitService : IProjectVisitService
{
    private readonly IProjectVisitRepository _repository;

    public ProjectVisitService(IProjectVisitRepository repository) => _repository = repository;

    public async Task<CreateProjectVisitResult> CreateAsync(CreateProjectVisitCommand command)
    {
        if (command == null || string.IsNullOrWhiteSpace(command.ProjectExternalId))
            return Failure("El proyecto es requerido.");
        if (command.SellerUserId <= 0)
            return Failure("El vendedor autenticado es requerido.");
        if (command.VisitedAtUtc == default)
            return Failure("La fecha de visita es requerida.");
        if (command.Latitude is < -90 or > 90)
            return Failure("La latitud debe estar entre -90 y 90.");
        if (command.Longitude is < -180 or > 180)
            return Failure("La longitud debe estar entre -180 y 180.");
        if (string.IsNullOrWhiteSpace(command.Result) || command.Result.Trim().Length > 50)
            return Failure("El resultado de la visita es requerido y no puede superar 50 caracteres.");
        if (command.Notes?.Trim().Length > 2000)
            return Failure("Las observaciones no pueden superar 2000 caracteres.");

        return await _repository.CreateAsync(new CreateProjectVisit
        {
            ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.ProjectVisit),
            ProjectExternalId = command.ProjectExternalId.Trim(),
            VisitedAtUtc = command.VisitedAtUtc.UtcDateTime,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Notes = string.IsNullOrWhiteSpace(command.Notes) ? null : command.Notes.Trim(),
            Result = command.Result.Trim(),
            SellerUserId = command.SellerUserId
        });
    }

    public Task<GetProjectVisitsResult> GetAsync(GetProjectVisitsCommand command)
    {
        if (command == null || string.IsNullOrWhiteSpace(command.ProjectExternalId))
            return Task.FromResult(new GetProjectVisitsResult { Message = "El proyecto es requerido." });
        if (command.From.HasValue && command.To.HasValue && command.From > command.To)
            return Task.FromResult(new GetProjectVisitsResult { Message = "La fecha desde no puede ser posterior a la fecha hasta." });

        return _repository.GetAsync(command with
        {
            ProjectExternalId = command.ProjectExternalId.Trim(),
            SellerExternalId = string.IsNullOrWhiteSpace(command.SellerExternalId) ? null : command.SellerExternalId.Trim()
        });
    }

    private static CreateProjectVisitResult Failure(string message) => new() { Message = message };
}
