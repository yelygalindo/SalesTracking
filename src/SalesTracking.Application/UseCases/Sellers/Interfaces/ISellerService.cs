using SalesTracking.Application.UseCases.Sellers.Comands;
using SalesTracking.Application.UseCases.Sellers.Results;

namespace SalesTracking.Application.UseCases.Sellers.Interfaces;

public interface ISellerService
{
    Task<IReadOnlyList<SellerResult>> GetAsync(GetSellersCommand command);
}
