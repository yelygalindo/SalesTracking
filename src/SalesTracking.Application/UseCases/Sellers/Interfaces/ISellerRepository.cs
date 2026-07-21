using SalesTracking.Application.UseCases.Sellers.Results;

namespace SalesTracking.Application.UseCases.Sellers.Interfaces;

public interface ISellerRepository
{
    Task<IReadOnlyList<SellerResult>> GetActiveAsync(int companyId);
}
