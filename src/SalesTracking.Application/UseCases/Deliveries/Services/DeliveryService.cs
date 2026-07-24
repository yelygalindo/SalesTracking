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

            return await _deliveryRepository.GetAsync(command with
            {
                Page = page,
                PageSize = pageSize,
                ProjectExternalId = Normalize(command.ProjectExternalId),
                CustomerExternalId = Normalize(command.CustomerExternalId),
                SellerExternalId = Normalize(command.SellerExternalId),
                StatusId = command.StatusId > 0 ? command.StatusId : null
            });
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

            CreateDelivery delivery = new CreateDelivery
            {
                ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.Delivery),
                ProjectExternalId = command.ProjectExternalId.Trim(),
                StatusId = PendingStatusId,
                CommittedDateUtc = command.CommittedDateUtc,
                DeliveredDateUtc = null,
                Notes = command.Notes?.Trim(),
                CreatedByUserId = command.CreatedByUserId,
                Items = command.Items.Select(x => new CreateDeliveryItem
                {
                    ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.DeliveryItem),
                    ProductExternalId = x.ProductExternalId.Trim(),
                    Quantity = x.Quantity,
                    DeliveredQuantity = 0
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

            UpdateDelivery delivery = new UpdateDelivery
            {
                ExternalId = command.ExternalId.Trim(),
                ProjectExternalId = command.ProjectExternalId.Trim(),
                CommittedDateUtc = command.CommittedDateUtc,
                Notes = command.Notes?.Trim(),
                UpdatedByUserId = command.UpdatedByUserId,
                Items = command.Items.Select(x => new CreateDeliveryItem
                {
                    ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.DeliveryItem),
                    ProductExternalId = x.ProductExternalId.Trim(),
                    Quantity = x.Quantity,
                    DeliveredQuantity = 0
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
                DeliveredDateUtc = NormalizeDeliveredDate(command.StatusId, command.DeliveredDateUtc),
                ChangedByUserId = command.ChangedByUserId
            });
        }


        public async Task<ConfirmDeliveryReceiptResult> ConfirmReceiptAsync(ConfirmDeliveryReceiptCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.DeliveryExternalId))
            {
                return new ConfirmDeliveryReceiptResult
                {
                    Succeeded = false,
                    Message = "La entrega es requerida."
                };
            }

            IReadOnlyList<ConfirmDeliveryReceiptItemCommand> itemList = command.Items?.ToList()
                ?? new List<ConfirmDeliveryReceiptItemCommand>();

            if (itemList.Count == 0)
            {
                return new ConfirmDeliveryReceiptResult
                {
                    Succeeded = false,
                    Message = "Debe registrar al menos un item recibido."
                };
            }

            foreach (ConfirmDeliveryReceiptItemCommand item in itemList)
            {
                if (string.IsNullOrWhiteSpace(item.DeliveryItemExternalId))
                {
                    return new ConfirmDeliveryReceiptResult
                    {
                        Succeeded = false,
                        Message = "El item de la entrega es requerido."
                    };
                }

                if (item.ReceivedQuantity <= 0)
                {
                    return new ConfirmDeliveryReceiptResult
                    {
                        Succeeded = false,
                        Message = "La cantidad recibida debe ser mayor que cero."
                    };
                }
            }

            ConfirmDeliveryReceiptCommand normalizedCommand = new ConfirmDeliveryReceiptCommand
            {
                DeliveryExternalId = command.DeliveryExternalId.Trim(),
                ReceivedAtUtc = command.ReceivedAtUtc,
                Notes = command.Notes?.Trim(),
                CreatedByUserId = command.CreatedByUserId,
                Items = itemList
                    .GroupBy(x => x.DeliveryItemExternalId.Trim())
                    .Select(x => new ConfirmDeliveryReceiptItemCommand
                    {
                        DeliveryItemExternalId = x.Key,
                        ReceivedQuantity = x.Sum(item => item.ReceivedQuantity)
                    })
                    .ToList()
            };

            return await _deliveryRepository.ConfirmReceiptAsync(normalizedCommand);
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

            return await _deliveryRepository.DeleteAsync(command with { ExternalId = command.ExternalId.Trim() });
        }

        private static string? Normalize(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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

            return Validate(command.ProjectExternalId, command.Items);
        }

        private static CreateDeliveryResult? Validate(UpdateDeliveryCommand command)
        {
            return Validate(command.ProjectExternalId, command.Items);
        }

        private static CreateDeliveryResult? Validate(
            string projectExternalId,
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

            IReadOnlyList<CreateDeliveryItemCommand> itemList = items?.ToList() ?? new List<CreateDeliveryItemCommand>();
            if (itemList.Count == 0)
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "La entrega debe tener al menos un item."
                };
            }

            if (itemList
                .Where(x => !string.IsNullOrWhiteSpace(x.ProductExternalId))
                .GroupBy(x => x.ProductExternalId.Trim(), StringComparer.OrdinalIgnoreCase)
                .Any(x => x.Count() > 1))
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "No se puede repetir el mismo producto en una entrega."
                };
            }

            foreach (CreateDeliveryItemCommand item in itemList)
            {
                CreateDeliveryResult? itemValidation = ValidateItem(
                    item.ProductExternalId,
                    item.Quantity);

                if (itemValidation != null)
                    return itemValidation;
            }

            return null;
        }

        private static CreateDeliveryResult? Validate(
            string projectExternalId,
            IEnumerable<UpdateDeliveryItemCommand> items)
        {
            IReadOnlyList<CreateDeliveryItemCommand> mappedItems = (items ?? new List<UpdateDeliveryItemCommand>())
                .Select(x => new CreateDeliveryItemCommand
                {
                    ProductExternalId = x.ProductExternalId,
                    Quantity = x.Quantity
                })
                .ToList();

            return Validate(projectExternalId, mappedItems);
        }

        private static CreateDeliveryResult? ValidateItem(
            string productExternalId,
            decimal quantity)
        {
            if (string.IsNullOrWhiteSpace(productExternalId))
            {
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "El producto del item es requerido."
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

            return null;
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
