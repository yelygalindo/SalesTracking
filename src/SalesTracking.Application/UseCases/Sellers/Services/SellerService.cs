using SalesTracking.Application.UseCases.Sellers.Comands;
using SalesTracking.Application.UseCases.Sellers.Interfaces;
using SalesTracking.Application.UseCases.Sellers.Results;

namespace SalesTracking.Application.UseCases.Sellers.Services;

public sealed class SellerService : ISellerService
{
    private readonly ISellerRepository _repository;

    public SellerService(ISellerRepository repository) => _repository = repository;

    public Task<IReadOnlyList<SellerResult>> GetAsync(GetSellersCommand command)
    {
        if (command == null || command.CompanyId <= 0)
            return Task.FromResult<IReadOnlyList<SellerResult>>([]);

        return _repository.GetActiveAsync(command.CompanyId);
    }
}
