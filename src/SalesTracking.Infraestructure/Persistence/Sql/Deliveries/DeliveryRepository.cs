using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Deliveries.Comands;
using SalesTracking.Application.UseCases.Deliveries.Interfaces;
using SalesTracking.Application.UseCases.Deliveries.Models;
using SalesTracking.Application.UseCases.Deliveries.Results;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.Deliveries.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Deliveries.Rows;
using SalesTracking.Infrastructure.Persistence.Sql.ProjectTimeline;
using System.Data;
using System.Text.Json;

namespace SalesTracking.Infrastructure.Persistence.Sql.Deliveries
{
    public sealed class DeliveryRepository : IDeliveryRepository
    {
        private const int PendingStatusId = 1;
        private const int PartialStatusId = 2;
        private const int DeliveredStatusId = 3;
        private const string RelatedEntityType = "Delivery";

        private readonly DatabaseSettings _databaseOptions;
        private readonly ICurrentUser _currentUser;

        public DeliveryRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private int CompanyId => _currentUser.CompanyId;
        private bool IsSeller => _currentUser.Roles.Contains("seller", StringComparer.OrdinalIgnoreCase);

        private IDbConnection CreateConnection() =>
            new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<IReadOnlyList<DeliveryStatusResult>> GetStatusesAsync()
        {
            using IDbConnection connection = CreateConnection();

            var rows = await connection.QueryAsync<DeliveryStatusRow>(
                DeliveryRepositoryQueries.GetStatuses);

            return rows.Select(x => x.ToResult()).ToList();
        }

        public async Task<DeliveryPagedList> GetAsync(GetDeliveriesCommand command)
        {
            using IDbConnection connection = CreateConnection();

            var rows = (await connection.QueryAsync<DeliveryRow>(
                DeliveryRepositoryQueries.Get,
                new
                {
                    Offset = (command.Page - 1) * command.PageSize,
                    command.PageSize,
                    command.ProjectExternalId,
                    command.CustomerExternalId,
                    SellerExternalId = IsSeller ? _currentUser.UserExternalId : command.SellerExternalId,
                    command.StatusId,
                    FromUtc = command.From?.UtcDateTime,
                    ToUtc = command.To?.UtcDateTime,
                    command.Overdue,
                    CompanyId
                })).ToList();

            var items = await GetItemsAsync(connection, rows.Select(x => x.Id).ToList());
            int totalItems = rows.FirstOrDefault()?.TotalCount ?? 0;

            return new DeliveryPagedList
            {
                Items = rows.Select(x => x.ToResult(GetItemsForDelivery(items, x.Id))).ToList(),
                Page = command.Page,
                PageSize = command.PageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0
                    ? 0
                    : (int)Math.Ceiling(totalItems / (double)command.PageSize)
            };
        }

        public async Task<DeliveryResult?> GetByExternalIdAsync(string externalId)
        {
            using IDbConnection connection = CreateConnection();

            DeliveryRow? row = await connection.QuerySingleOrDefaultAsync<DeliveryRow>(
                DeliveryRepositoryQueries.GetByExternalId,
                new { ExternalId = externalId, CompanyId });

            if (row == null)
                return null;

            var items = await GetItemsAsync(connection, new[] { row.Id });
            return row.ToResult(GetItemsForDelivery(items, row.Id));
        }

        public async Task<CreateDeliveryResult> CreateAsync(CreateDelivery delivery)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                ProjectDeliveryRow? project = await connection.QuerySingleOrDefaultAsync<ProjectDeliveryRow>(
                    DeliveryRepositoryQueries.GetProjectInternalIdByExternalId,
                    new
                    {
                        ExternalId = delivery.ProjectExternalId,
                        CompanyId,
                        SellerUserId = IsSeller ? _currentUser.UserId : (int?)null
                    },
                    transaction);

                if (project == null)
                    return RollbackCreate(transaction, "Proyecto no encontrado.", true);

                int deliveryId = await connection.QuerySingleAsync<int>(
                    DeliveryRepositoryQueries.Insert,
                    new
                    {
                        delivery.ExternalId,
                        ProjectId = project.Id,
                        SellerId = project.SellerId,
                        delivery.StatusId,
                        delivery.CommittedDateUtc,
                        delivery.DeliveredDateUtc,
                        delivery.Notes,
                        CompanyId
                    },
                    transaction);

                foreach (CreateDeliveryItem item in delivery.Items)
                {
                    ProductDeliveryRow? product = await GetProductAsync(
                        connection, transaction, item.ProductExternalId);

                    if (product == null)
                        return RollbackCreate(transaction, "Producto no encontrado.", true);

                    await InsertItemAsync(connection, transaction, deliveryId, item, product.Id, product.UnitId);
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = project.Id,
                        EventTypeId = ProjectTimelineEventTypeIds.DeliveryCreated,
                        Title = "Entrega creada",
                        Description = "Se creo una entrega para el proyecto.",
                        CreatedByUserId = delivery.CreatedByUserId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = deliveryId
                    });

                transaction.Commit();

                return new CreateDeliveryResult
                {
                    Succeeded = true,
                    Id = delivery.ExternalId,
                    Message = "Entrega creada correctamente."
                };
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return new CreateDeliveryResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al crear la entrega."
                };
            }
        }

        public async Task<UpdateDeliveryResult> UpdateAsync(UpdateDelivery delivery)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                DeliveryInternalRow? existing = await connection.QuerySingleOrDefaultAsync<DeliveryInternalRow>(
                    DeliveryRepositoryQueries.GetDeliveryInternalByExternalId,
                    new { delivery.ExternalId, CompanyId },
                    transaction);

                if (existing == null)
                    return RollbackUpdate(transaction, "Entrega no encontrada.", true);

                ProjectDeliveryRow? project = await connection.QuerySingleOrDefaultAsync<ProjectDeliveryRow>(
                    DeliveryRepositoryQueries.GetProjectInternalIdByExternalId,
                    new
                    {
                        ExternalId = delivery.ProjectExternalId,
                        CompanyId,
                        SellerUserId = IsSeller ? _currentUser.UserId : (int?)null
                    },
                    transaction);

                if (project == null)
                    return RollbackUpdate(transaction, "Proyecto no encontrado.", true);

                var existingItems = (await connection.QueryAsync<ExistingDeliveryItemRow>(
                    DeliveryRepositoryQueries.GetExistingItemsForUpdate,
                    new { DeliveryId = existing.Id, CompanyId },
                    transaction)).ToList();
                bool hasReceipts = existingItems.Any(x => x.DeliveredQuantity > 0);

                if (hasReceipts && project.Id != existing.ProjectId)
                    return RollbackUpdate(
                        transaction,
                        "No se puede cambiar el proyecto de una entrega que ya tiene recepciones.",
                        false);

                var preparedItems = new List<(CreateDeliveryItem Item, ProductDeliveryRow Product)>();
                foreach (CreateDeliveryItem item in delivery.Items)
                {
                    ProductDeliveryRow? product = await GetProductAsync(
                        connection, transaction, item.ProductExternalId);

                    if (product == null)
                        return RollbackUpdate(transaction, "Producto no encontrado.", true);

                    decimal deliveredQuantity = existingItems
                        .Where(x => x.ProductId == product.Id)
                        .Sum(x => x.DeliveredQuantity);

                    if (item.Quantity < deliveredQuantity)
                        return RollbackUpdate(transaction, "La cantidad no puede ser menor que la cantidad ya entregada.", false);

                    item.DeliveredQuantity = deliveredQuantity;
                    preparedItems.Add((item, product));
                }

                var requestedProductIds = preparedItems.Select(x => x.Product.Id).ToHashSet();
                if (hasReceipts)
                {
                    bool itemsChanged =
                        requestedProductIds.Count != existingItems.Count ||
                        existingItems.Any(existingItem =>
                            !requestedProductIds.Contains(existingItem.ProductId) ||
                            preparedItems.Single(x => x.Product.Id == existingItem.ProductId).Item.Quantity != existingItem.Quantity);

                    if (itemsChanged)
                    {
                        return RollbackUpdate(
                            transaction,
                            "No se pueden modificar los productos ni las cantidades de una entrega que ya tiene recepciones.",
                            false);
                    }
                }

                int statusId = ResolveStatusId(preparedItems.Select(x => new DeliveryQuantityRow
                {
                    Quantity = x.Item.Quantity,
                    DeliveredQuantity = x.Item.DeliveredQuantity
                }).ToList());
                DateTime? deliveredDateUtc = statusId == DeliveredStatusId
                    ? existing.DeliveredDateUtc ?? DateTime.UtcNow
                    : null;

                int affectedRows = await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.Update,
                    new
                    {
                        existing.Id,
                        ProjectId = project.Id,
                        SellerId = project.SellerId,
                        StatusId = statusId,
                        delivery.CommittedDateUtc,
                        DeliveredDateUtc = deliveredDateUtc,
                        delivery.Notes,
                        CompanyId
                    },
                    transaction);

                if (affectedRows == 0)
                    return RollbackUpdate(transaction, "Entrega no encontrada.", true);

                if (!hasReceipts)
                {
                    await connection.ExecuteAsync(
                        DeliveryRepositoryQueries.SoftDeleteItems,
                        new { DeliveryId = existing.Id, CompanyId },
                        transaction);

                    foreach ((CreateDeliveryItem item, ProductDeliveryRow product) in preparedItems)
                    {
                        await InsertItemAsync(connection, transaction, existing.Id, item, product.Id, product.UnitId);
                    }
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = project.Id,
                        EventTypeId = ProjectTimelineEventTypeIds.DeliveryUpdated,
                        Title = "Entrega actualizada",
                        Description = "Entrega del proyecto actualizada.",
                        CreatedByUserId = delivery.UpdatedByUserId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = existing.Id
                    });

                transaction.Commit();

                return new UpdateDeliveryResult
                {
                    Succeeded = true,
                    Message = "Entrega actualizada correctamente."
                };
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return new UpdateDeliveryResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al actualizar la entrega."
                };
            }
        }

        public async Task<ChangeDeliveryStatusResult> ChangeStatusAsync(ChangeDeliveryStatusCommand command)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                DeliveryInternalRow? delivery = await connection.QuerySingleOrDefaultAsync<DeliveryInternalRow>(
                    DeliveryRepositoryQueries.GetDeliveryInternalByExternalId,
                    new { command.ExternalId, CompanyId },
                    transaction);

                if (delivery == null)
                    return RollbackChangeStatus(transaction, "Entrega no encontrada.", true);

                bool statusExists = await connection.QuerySingleAsync<int>(
                    DeliveryRepositoryQueries.StatusExists,
                    new { command.StatusId },
                    transaction) > 0;

                if (!statusExists)
                    return RollbackChangeStatus(transaction, "Estado de entrega no encontrado.", true);

                var quantities = (await connection.QueryAsync<DeliveryQuantityRow>(
                    DeliveryRepositoryQueries.GetQuantitiesByDeliveryId,
                    new { DeliveryId = delivery.Id, CompanyId },
                    transaction)).ToList();

                int expectedStatusId = ResolveStatusId(quantities);
                if (command.StatusId != expectedStatusId)
                {
                    return RollbackChangeStatus(
                        transaction,
                        "El estado solicitado no coincide con las cantidades entregadas.",
                        false);
                }

                await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.ChangeStatus,
                    new
                    {
                        delivery.Id,
                        command.StatusId,
                        DeliveredDateUtc = command.StatusId == DeliveredStatusId
                            ? command.DeliveredDateUtc ?? DateTime.UtcNow
                            : (DateTime?)null,
                        CompanyId
                    },
                    transaction);

                string previousStatusName = await GetStatusNameAsync(connection, transaction, delivery.StatusId);
                string newStatusName = await GetStatusNameAsync(connection, transaction, command.StatusId);

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = delivery.ProjectId,
                        EventTypeId = ProjectTimelineEventTypeIds.DeliveryStatusChanged,
                        Title = "Estado de entrega actualizado",
                        Description = $"Estado de entrega actualizado de {previousStatusName} a {newStatusName}.",
                        CreatedByUserId = command.ChangedByUserId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = delivery.Id
                    });

                if (delivery.StatusId != DeliveredStatusId && command.StatusId == DeliveredStatusId)
                {
                    await ProjectTimelineWriter.InsertAsync(
                        connection,
                        transaction,
                        new ProjectTimelineEvent
                        {
                            ProjectId = delivery.ProjectId,
                            EventTypeId = ProjectTimelineEventTypeIds.DeliveryCompleted,
                            Title = "Entrega completada",
                            Description = "La entrega fue completada.",
                            OccurredAtUtc = command.DeliveredDateUtc ?? DateTime.UtcNow,
                            CreatedByUserId = command.ChangedByUserId,
                            RelatedEntityType = RelatedEntityType,
                            RelatedEntityId = delivery.Id
                        });
                }

                transaction.Commit();

                return new ChangeDeliveryStatusResult
                {
                    Succeeded = true,
                    Message = "Estado de entrega actualizado correctamente."
                };
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return new ChangeDeliveryStatusResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al actualizar el estado de la entrega."
                };
            }
        }


        public async Task<ConfirmDeliveryReceiptResult> ConfirmReceiptAsync(ConfirmDeliveryReceiptCommand command)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                DeliveryInternalRow? delivery = await connection.QuerySingleOrDefaultAsync<DeliveryInternalRow>(
                    DeliveryRepositoryQueries.GetDeliveryInternalByExternalId,
                    new { ExternalId = command.DeliveryExternalId, CompanyId },
                    transaction);

                if (delivery == null)
                    return RollbackConfirmReceipt(transaction, "Entrega no encontrada.", true);

                decimal totalReceivedQuantity = 0;
                foreach (ConfirmDeliveryReceiptItemCommand item in command.Items)
                {
                    DeliveryReceiptItemRow? deliveryItem = await connection.QuerySingleOrDefaultAsync<DeliveryReceiptItemRow>(
                        DeliveryRepositoryQueries.GetReceiptItemByExternalId,
                        new
                        {
                            ExternalId = item.DeliveryItemExternalId,
                            DeliveryId = delivery.Id,
                            CompanyId
                        },
                        transaction);

                    if (deliveryItem == null)
                        return RollbackConfirmReceipt(transaction, "Item de entrega no encontrado.", true);

                    decimal newDeliveredQuantity = deliveryItem.DeliveredQuantity + item.ReceivedQuantity;
                    if (newDeliveredQuantity > deliveryItem.Quantity)
                    {
                        return RollbackConfirmReceipt(
                            transaction,
                            "La cantidad acumulada no puede superar la cantidad comprometida.",
                            false);
                    }

                    int affectedRows = await connection.ExecuteAsync(
                        DeliveryRepositoryQueries.UpdateItemDeliveredQuantity,
                        new
                        {
                            deliveryItem.Id,
                            DeliveryId = delivery.Id,
                            DeliveredQuantity = newDeliveredQuantity,
                            CompanyId
                        },
                        transaction);

                    if (affectedRows == 0)
                        return RollbackConfirmReceipt(transaction, "No se pudo actualizar el item de la entrega.", false);

                    totalReceivedQuantity += item.ReceivedQuantity;
                }

                var quantities = (await connection.QueryAsync<DeliveryQuantityRow>(
                    DeliveryRepositoryQueries.GetQuantitiesByDeliveryId,
                    new { DeliveryId = delivery.Id, CompanyId },
                    transaction)).ToList();

                int statusId = ResolveStatusId(quantities);
                DateTime? deliveredDateUtc = statusId == DeliveredStatusId
                    ? command.ReceivedAtUtc
                    : (DateTime?)null;

                await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.UpdateDeliveryReceiptState,
                    new
                    {
                        delivery.Id,
                        StatusId = statusId,
                        DeliveredDateUtc = deliveredDateUtc,
                        CompanyId
                    },
                    transaction);

                string metadataJson = JsonSerializer.Serialize(new
                {
                    command.ReceivedAtUtc,
                    command.Notes,
                    Items = command.Items.Select(x => new
                    {
                        x.DeliveryItemExternalId,
                        x.ReceivedQuantity
                    }).ToList()
                });

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = delivery.ProjectId,
                        EventTypeId = ProjectTimelineEventTypeIds.DeliveryReceiptConfirmed,
                        Title = "Recepcion de entrega registrada",
                        Description = string.IsNullOrWhiteSpace(command.Notes)
                            ? $"Se registro la recepcion de {totalReceivedQuantity} unidades."
                            : command.Notes,
                        OccurredAtUtc = command.ReceivedAtUtc,
                        CreatedByUserId = command.CreatedByUserId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = delivery.Id,
                        MetadataJson = metadataJson
                    });

                if (delivery.StatusId != DeliveredStatusId && statusId == DeliveredStatusId)
                {
                    await ProjectTimelineWriter.InsertAsync(
                        connection,
                        transaction,
                        new ProjectTimelineEvent
                        {
                            ProjectId = delivery.ProjectId,
                            EventTypeId = ProjectTimelineEventTypeIds.DeliveryCompleted,
                            Title = "Entrega completada",
                            Description = "La recepción completó todas las cantidades comprometidas.",
                            OccurredAtUtc = command.ReceivedAtUtc,
                            CreatedByUserId = command.CreatedByUserId,
                            RelatedEntityType = RelatedEntityType,
                            RelatedEntityId = delivery.Id,
                            MetadataJson = metadataJson
                        });
                }

                transaction.Commit();

                return new ConfirmDeliveryReceiptResult
                {
                    Succeeded = true,
                    Message = "Recepcion de entrega registrada correctamente."
                };
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return new ConfirmDeliveryReceiptResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al registrar la recepcion de la entrega."
                };
            }
        }
        public async Task<DeleteDeliveryResult> DeleteAsync(DeleteDeliveryCommand command)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                DeliveryInternalRow? delivery = await connection.QuerySingleOrDefaultAsync<DeliveryInternalRow>(
                    DeliveryRepositoryQueries.GetDeliveryInternalByExternalId,
                    new { command.ExternalId, CompanyId },
                    transaction);

                if (delivery == null)
                    return RollbackDelete(transaction, "Entrega no encontrada.", true);

                int affectedRows = await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.DeleteDelivery,
                    new { command.ExternalId, CompanyId },
                    transaction);

                if (affectedRows == 0)
                    return RollbackDelete(transaction, "Entrega no encontrada.", true);

                await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.SoftDeleteItems,
                    new { DeliveryId = delivery.Id, CompanyId },
                    transaction);

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = delivery.ProjectId,
                        EventTypeId = ProjectTimelineEventTypeIds.DeliveryDeleted,
                        Title = "Entrega eliminada",
                        Description = "Entrega eliminada del proyecto.",
                        CreatedByUserId = command.DeletedByUserId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = delivery.Id
                    });

                transaction.Commit();

                return new DeleteDeliveryResult
                {
                    Succeeded = true,
                    Message = "Entrega eliminada correctamente."
                };
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return new DeleteDeliveryResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al eliminar la entrega."
                };
            }
        }

        private async Task<IReadOnlyList<DeliveryItemRow>> GetItemsAsync(
            IDbConnection connection,
            IReadOnlyCollection<int> deliveryIds)
        {
            if (deliveryIds.Count == 0)
                return new List<DeliveryItemRow>();

            var rows = await connection.QueryAsync<DeliveryItemRow>(
                DeliveryRepositoryQueries.GetItemsByDeliveryIds,
                new { DeliveryIds = deliveryIds, CompanyId });

            return rows.ToList();
        }

        private static IReadOnlyList<DeliveryItemResult> GetItemsForDelivery(
            IReadOnlyList<DeliveryItemRow> items,
            int deliveryId)
        {
            return items
                .Where(x => x.DeliveryId == deliveryId)
                .Select(x => x.ToResult())
                .ToList();
        }

        private async Task<ProductDeliveryRow?> GetProductAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            string externalId)
        {
            return await connection.QuerySingleOrDefaultAsync<ProductDeliveryRow>(
                DeliveryRepositoryQueries.GetProductInternalIdByExternalId,
                new { ExternalId = externalId, CompanyId },
                transaction);
        }

        private async Task InsertItemAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int deliveryId,
            CreateDeliveryItem item,
            int productId,
            int unitId)
        {
            await connection.ExecuteAsync(
                DeliveryRepositoryQueries.InsertItem,
                new
                {
                    item.ExternalId,
                    DeliveryId = deliveryId,
                    ProductId = productId,
                    UnitId = unitId,
                    item.Quantity,
                    item.DeliveredQuantity,
                    CompanyId
                },
                transaction);
        }

        private static int ResolveStatusId(IReadOnlyList<DeliveryQuantityRow> quantities)
        {
            if (quantities.All(x => x.DeliveredQuantity == 0))
                return PendingStatusId;

            if (quantities.All(x => x.DeliveredQuantity == x.Quantity))
                return DeliveredStatusId;

            return PartialStatusId;
        }

        private static async Task<string> GetStatusNameAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int statusId)
        {
            return await connection.QuerySingleOrDefaultAsync<string>(
                DeliveryRepositoryQueries.GetStatusName,
                new { StatusId = statusId },
                transaction) ?? string.Empty;
        }

        private static CreateDeliveryResult RollbackCreate(IDbTransaction transaction, string message, bool notFound)
        {
            transaction.Rollback();
            return new CreateDeliveryResult
            {
                Succeeded = false,
                NotFound = notFound,
                Message = message
            };
        }

        private static UpdateDeliveryResult RollbackUpdate(IDbTransaction transaction, string message, bool notFound)
        {
            transaction.Rollback();
            return new UpdateDeliveryResult
            {
                Succeeded = false,
                NotFound = notFound,
                Message = message
            };
        }

        private static ChangeDeliveryStatusResult RollbackChangeStatus(IDbTransaction transaction, string message, bool notFound)
        {
            transaction.Rollback();
            return new ChangeDeliveryStatusResult
            {
                Succeeded = false,
                NotFound = notFound,
                Message = message
            };
        }


        private static ConfirmDeliveryReceiptResult RollbackConfirmReceipt(IDbTransaction transaction, string message, bool notFound)
        {
            transaction.Rollback();
            return new ConfirmDeliveryReceiptResult
            {
                Succeeded = false,
                NotFound = notFound,
                Message = message
            };
        }
        private static DeleteDeliveryResult RollbackDelete(IDbTransaction transaction, string message, bool notFound)
        {
            transaction.Rollback();
            return new DeleteDeliveryResult
            {
                Succeeded = false,
                NotFound = notFound,
                Message = message
            };
        }

        private sealed class DeliveryQuantityRow
        {
            public decimal Quantity { get; set; }
            public decimal DeliveredQuantity { get; set; }
        }

        private sealed class ProjectDeliveryRow
        {
            public int Id { get; set; }
            public int SellerId { get; set; }
        }

        private sealed class ProductDeliveryRow
        {
            public int Id { get; set; }
            public int UnitId { get; set; }
        }

        private sealed class ExistingDeliveryItemRow
        {
            public int ProductId { get; set; }
            public decimal Quantity { get; set; }
            public decimal DeliveredQuantity { get; set; }
        }
    }
}
