namespace SalesTracking.Application.Common.Interfaces;

public interface IResourceOwnershipRepository
{
    Task<bool> IsAssignedToSellerAsync(string resource, string externalId, int companyId, int sellerUserId);
}
