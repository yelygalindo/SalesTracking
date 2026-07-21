using SalesTracking.Application.UseCases.Deliveries.Comands;
using SalesTracking.Application.UseCases.Deliveries.Models;
using SalesTracking.Application.UseCases.Deliveries.Results;

namespace SalesTracking.Application.UseCases.Deliveries.Interfaces
{
    public interface IDeliveryRepository
    {
        Task<IReadOnlyList<DeliveryStatusResult>> GetStatusesAsync();
        Task<DeliveryPagedList> GetAsync(GetDeliveriesCommand command);
        Task<DeliveryResult?> GetByExternalIdAsync(string externalId);
        Task<CreateDeliveryResult> CreateAsync(CreateDelivery delivery);
        Task<UpdateDeliveryResult> UpdateAsync(UpdateDelivery delivery);
        Task<ChangeDeliveryStatusResult> ChangeStatusAsync(ChangeDeliveryStatusCommand command);
        Task<DeleteDeliveryResult> DeleteAsync(string externalId);
    }
}
