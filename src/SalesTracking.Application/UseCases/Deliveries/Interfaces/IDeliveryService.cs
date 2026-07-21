using SalesTracking.Application.UseCases.Deliveries.Comands;
using SalesTracking.Application.UseCases.Deliveries.Models;
using SalesTracking.Application.UseCases.Deliveries.Results;

namespace SalesTracking.Application.UseCases.Deliveries.Interfaces
{
    public interface IDeliveryService
    {
        Task<IReadOnlyList<DeliveryStatusResult>> GetStatusesAsync();
        Task<DeliveryPagedList> GetAsync(GetDeliveriesCommand command);
        Task<DeliveryResult?> GetByExternalIdAsync(GetDeliveryByExternalIdCommand command);
        Task<CreateDeliveryResult> CreateAsync(CreateDeliveryCommand command);
        Task<UpdateDeliveryResult> UpdateAsync(UpdateDeliveryCommand command);
        Task<ChangeDeliveryStatusResult> ChangeStatusAsync(ChangeDeliveryStatusCommand command);
        Task<DeleteDeliveryResult> DeleteAsync(DeleteDeliveryCommand command);
    }
}
