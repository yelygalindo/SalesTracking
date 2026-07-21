using SalesTracking.Application.UseCases.CustomerTimeline.Comands;
using SalesTracking.Application.UseCases.CustomerTimeline.Interfaces;
using SalesTracking.Application.UseCases.CustomerTimeline.Results;

namespace SalesTracking.Application.UseCases.CustomerTimeline.Services;

public sealed class CustomerTimelineService : ICustomerTimelineService
{
    private readonly ICustomerTimelineRepository _repository;

    public CustomerTimelineService(ICustomerTimelineRepository repository)
    {
        _repository = repository;
    }

    public Task<GetCustomerTimelineResult> GetAsync(GetCustomerTimelineCommand command)
    {
        if (command == null || string.IsNullOrWhiteSpace(command.CustomerExternalId))
        {
            return Task.FromResult(new GetCustomerTimelineResult
            {
                Succeeded = false,
                Message = "El cliente es requerido."
            });
        }

        int page = command.Page <= 0 ? 1 : command.Page;
        int pageSize = command.PageSize <= 0 ? 20 : Math.Min(command.PageSize, 100);

        return _repository.GetAsync(new GetCustomerTimelineCommand(
            command.CustomerExternalId.Trim(), page, pageSize));
    }
}
