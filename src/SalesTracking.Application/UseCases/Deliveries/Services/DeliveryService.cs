using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.Deliveries.Comands;
using SalesTracking.Application.UseCases.Deliveries.Interfaces;
using SalesTracking.Application.UseCases.Deliveries.Models;
using SalesTracking.Application.UseCases.Deliveries.Results;

namespace SalesTracking.Application.UseCases.Deliveries.Services
{
    public sealed class DeliveryService : IDeliveryService
    {
        private const int PendingStatusId = 1;
        private const int PartialStatusId = 2;
        private const int DeliveredStatusId = 3;

        private readonly IDeliveryRepository _deliveryRepository;

        public DeliveryService(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<IReadOnlyList<DeliveryStatusResult>> GetStatusesAsync()
        {
            return await _deliveryRepository.GetStatusesAsync();
        }

        public async Task<DeliveryPagedList> GetAsync(GetDeliveriesCommand command)
        {
            int page = command.Page <= 0 ? 1 : command.Page;
            int pageSize = command.PageSize <= 0 ? 20 : command.PageSize;

            if (pageSize > 100)
                pageSize = 100;

            return await _deliveryRepository.GetAsync(new GetDeliveriesCommand(page, pageSize));
        }

        public async Task<DeliveryResult?> GetByExternalIdAsync(GetDeliveryByExternalIdCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
                return null;

            return await _deliveryRepository.GetByExternalIdAsync(command.ExternalId.Trim());
        }

        public async Task<CreateDeliveryResult> CreateAsync(CreateDeliveryCommand command)
        {
            CreateDeliveryResult? validation = Validate(command);
            if (validation != null)
                return validation;

            int statusId = ResolveStatusId(command.Items);
            CreateDelivery delivery = new CreateDelivery
            {
                ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.Delivery),
                ProjectExternalId = command.ProjectExternalId.Trim(),
                SellerExternalId = command.SellerExternalId.Trim(),
                StatusId = statusId,
                CommittedDateUtc = command.CommittedDateUtc,
                DeliveredDateUtc = NormalizeDeliveredDate(statusId, command.DeliveredDateUtc),
                Notes = command.Notes?.Trim(),
                Items = command.Items.Select(x => new CreateDeliveryItem
                {
                    ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.DeliveryItem),
                    ProductExternalId = x.ProductExternalId.Trim(),
                    UnitExternalId = x.UnitExternalId.Trim(),
                    Quantity = x.Quantity,
                    DeliveredQuantity = x.DeliveredQuantity
                }).ToList()
            };

            return await _deliveryRepository.CreateAsync(delivery);
        }

        public async Task<UpdateDeliveryResult> UpdateAsync(UpdateDeliveryCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new UpdateDeliveryResult
                {
                    Succeeded = false,
                    Message = "La entrega es requerida."
                };
            }

            CreateDeliveryResult? validation = Validate(command);
            if (validation != null)
            {
                return new UpdateDeliveryResult
                {
                    Succeeded = false,
                    Message = validation.Message
                };
            }

            int statusId = ResolveStatusId(command.Items);
            UpdateDelivery delivery = new UpdateDelivery
            {
                ExternalId = command.ExternalId.Trim(),
                ProjectExternalId = command.ProjectExternalId.Trim(),
                SellerExternalId = command.SellerExternalId.Trim(),
                StatusId = statusId,
                CommittedDateUtc = command.CommittedDateUtc,
                DeliveredDateUtc = NormalizeDeliveredDate(statusId, command.DeliveredDateUtc),
                Notes = command.Notes?.Trim(),
                Items = command.Items.Select(x => new CreateDeliveryItem
                {
                    ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.DeliveryItem),
                    ProductExternalId = x.ProductExternalId.Trim(),
                    UnitExternalId = x.UnitExternalId.Trim(),
                    Quantity = x.Quantity,
                    DeliveredQuantity = x.DeliveredQuantity
                }).ToList()
            };

            return await _deliveryRepository.UpdateAsync(delivery);
        }

        public async Task<ChangeDeliveryStatusResult> ChangeStatusAsync(ChangeDeliveryStatusCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new ChangeDeliveryStatusResult
                {
                    Succeeded = false,
                    Message = "La entrega es requerida."
                };
            }

            if (!IsKnownStatus(command.StatusId))
            {
                return new ChangeDeliveryStatusResult
                {
                    Succeeded = false,
                    Message = "El estado de la entrega no es valido."
                };
            }

            return await _deliveryRepository.ChangeStatusAsync(new ChangeDeliveryStatusCommand
            {
                ExternalId = command.ExternalId.Trim(),
                StatusId = command.StatusId,
                DeliveredDateUtc = NormalizeDeliveredDate(command.StatusId, command.DeliveredDateUtc)
            });
        }

        public async Task<DeleteDeliveryResult> DeleteAsync(DeleteDeliveryCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return new DeleteDeliveryResult
                {
                    Succeeded = false,
                    Message = "La entrega es requerida."
                };
            }

            return await _deliveryRepository.DeleteAsync(command.ExternalId.Trim());
        }

        private static CreateDeliveryResult? Validate(CreateDeliveryCommand command)
        {
            if (command == null)
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "La entrega es requerida."
                };
            }

            return Validate(command.ProjectExternalId, command.SellerExternalId, command.Items);
        }

        private static CreateDeliveryResult? Validate(UpdateDeliveryCommand command)
        {
            return Validate(command.ProjectExternalId, command.SellerExternalId, command.Items);
        }

        private static CreateDeliveryResult? Validate(
            string projectExternalId,
            string sellerExternalId,
            IEnumerable<CreateDeliveryItemCommand> items)
        {
            if (string.IsNullOrWhiteSpace(projectExternalId))
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "El proyecto es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(sellerExternalId))
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "El vendedor es requerido."
                };
            }

            IReadOnlyList<CreateDeliveryItemCommand> itemList = items?.ToList() ?? new List<CreateDeliveryItemCommand>();
            if (itemList.Count == 0)
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "La entrega debe tener al menos un item."
                };
            }

            foreach (CreateDeliveryItemCommand item in itemList)
            {
                CreateDeliveryResult? itemValidation = ValidateItem(
                    item.ProductExternalId,
                    item.UnitExternalId,
                    item.Quantity,
                    item.DeliveredQuantity);

                if (itemValidation != null)
                    return itemValidation;
            }

            return null;
        }

        private static CreateDeliveryResult? Validate(
            string projectExternalId,
            string sellerExternalId,
            IEnumerable<UpdateDeliveryItemCommand> items)
        {
            IReadOnlyList<CreateDeliveryItemCommand> mappedItems = (items ?? new List<UpdateDeliveryItemCommand>())
                .Select(x => new CreateDeliveryItemCommand
                {
                    ProductExternalId = x.ProductExternalId,
                    UnitExternalId = x.UnitExternalId,
                    Quantity = x.Quantity,
                    DeliveredQuantity = x.DeliveredQuantity
                })
                .ToList();

            return Validate(projectExternalId, sellerExternalId, mappedItems);
        }

        private static CreateDeliveryResult? ValidateItem(
            string productExternalId,
            string unitExternalId,
            decimal quantity,
            decimal deliveredQuantity)
        {
            if (string.IsNullOrWhiteSpace(productExternalId))
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "El producto del item es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(unitExternalId))
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "La unidad del item es requerida."
                };
            }

            if (quantity <= 0)
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "La cantidad debe ser mayor que cero."
                };
            }

            if (deliveredQuantity < 0 || deliveredQuantity > quantity)
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "La cantidad entregada no es valida."
                };
            }

            return null;
        }

        private static int ResolveStatusId(IEnumerable<CreateDeliveryItemCommand> items)
        {
            IReadOnlyList<CreateDeliveryItemCommand> itemList = items.ToList();

            if (itemList.All(x => x.DeliveredQuantity == 0))
                return PendingStatusId;

            if (itemList.All(x => x.DeliveredQuantity == x.Quantity))
                return DeliveredStatusId;

            return PartialStatusId;
        }

        private static int ResolveStatusId(IEnumerable<UpdateDeliveryItemCommand> items)
        {
            return ResolveStatusId(items.Select(x => new CreateDeliveryItemCommand
            {
                Quantity = x.Quantity,
                DeliveredQuantity = x.DeliveredQuantity
            }));
        }

        private static DateTime? NormalizeDeliveredDate(int statusId, DateTime? deliveredDateUtc)
        {
            if (statusId == DeliveredStatusId)
                return deliveredDateUtc ?? DateTime.UtcNow;

            return null;
        }

        private static bool IsKnownStatus(int statusId)
        {
            return statusId == PendingStatusId
                || statusId == PartialStatusId
                || statusId == DeliveredStatusId;
        }
    }
}
