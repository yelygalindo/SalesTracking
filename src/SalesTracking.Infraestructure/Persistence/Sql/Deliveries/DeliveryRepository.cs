using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.UseCases.Deliveries.Comands;
using SalesTracking.Application.UseCases.Deliveries.Interfaces;
using SalesTracking.Application.UseCases.Deliveries.Models;
using SalesTracking.Application.UseCases.Deliveries.Results;
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

        public DeliveryRepository(IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value
                ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

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
                    command.PageSize
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
                new { ExternalId = externalId });

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
                int? projectId = await GetInternalIdAsync(
                    connection,
                    transaction,
                    DeliveryRepositoryQueries.GetProjectInternalIdByExternalId,
                    delivery.ProjectExternalId);

                if (projectId == null)
                    return RollbackCreate(transaction, "Proyecto no encontrado.", true);

                int? sellerId = await GetInternalIdAsync(
                    connection,
                    transaction,
                    DeliveryRepositoryQueries.GetSellerInternalIdByExternalId,
                    delivery.SellerExternalId);

                if (sellerId == null)
                    return RollbackCreate(transaction, "Vendedor no encontrado.", true);

                int deliveryId = await connection.QuerySingleAsync<int>(
                    DeliveryRepositoryQueries.Insert,
                    new
                    {
                        delivery.ExternalId,
                        ProjectId = projectId.Value,
                        SellerId = sellerId.Value,
                        delivery.StatusId,
                        delivery.CommittedDateUtc,
                        delivery.DeliveredDateUtc,
                        delivery.Notes
                    },
                    transaction);

                foreach (CreateDeliveryItem item in delivery.Items)
                {
                    int? productId = await GetInternalIdAsync(
                        connection,
                        transaction,
                        DeliveryRepositoryQueries.GetProductInternalIdByExternalId,
                        item.ProductExternalId);

                    if (productId == null)
                        return RollbackCreate(transaction, "Producto no encontrado.", true);

                    int? unitId = await GetInternalIdAsync(
                        connection,
                        transaction,
                        DeliveryRepositoryQueries.GetUnitInternalIdByExternalId,
                        item.UnitExternalId);

                    if (unitId == null)
                        return RollbackCreate(transaction, "Unidad no encontrada.", true);

                    await InsertItemAsync(connection, transaction, deliveryId, item, productId.Value, unitId.Value);
                }

                await ProjectTimelineWriter.InsertAsync(
                    connection,
                    transaction,
                    new ProjectTimelineEvent
                    {
                        ProjectId = projectId.Value,
                        EventTypeId = ProjectTimelineEventTypeIds.DeliveryCreated,
                        Title = "Entrega creada",
                        Description = "Se creo una entrega para el proyecto.",
                        CreatedByUserId = sellerId.Value,
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
            catch
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
                    new { delivery.ExternalId },
                    transaction);

                if (existing == null)
                    return RollbackUpdate(transaction, "Entrega no encontrada.", true);

                int? projectId = await GetInternalIdAsync(
                    connection,
                    transaction,
                    DeliveryRepositoryQueries.GetProjectInternalIdByExternalId,
                    delivery.ProjectExternalId);

                if (projectId == null)
                    return RollbackUpdate(transaction, "Proyecto no encontrado.", true);

                int? sellerId = await GetInternalIdAsync(
                    connection,
                    transaction,
                    DeliveryRepositoryQueries.GetSellerInternalIdByExternalId,
                    delivery.SellerExternalId);

                if (sellerId == null)
                    return RollbackUpdate(transaction, "Vendedor no encontrado.", true);

                int affectedRows = await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.Update,
                    new
                    {
                        existing.Id,
                        ProjectId = projectId.Value,
                        SellerId = sellerId.Value,
                        delivery.StatusId,
                        delivery.CommittedDateUtc,
                        delivery.DeliveredDateUtc,
                        delivery.Notes
                    },
                    transaction);

                if (affectedRows == 0)
                    return RollbackUpdate(transaction, "Entrega no encontrada.", true);

                await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.SoftDeleteItems,
                    new { DeliveryId = existing.Id },
                    transaction);

                foreach (CreateDeliveryItem item in delivery.Items)
                {
                    int? productId = await GetInternalIdAsync(
                        connection,
                        transaction,
                        DeliveryRepositoryQueries.GetProductInternalIdByExternalId,
                        item.ProductExternalId);

                    if (productId == null)
                        return RollbackUpdate(transaction, "Producto no encontrado.", true);

                    int? unitId = await GetInternalIdAsync(
                        connection,
                        transaction,
                        DeliveryRepositoryQueries.GetUnitInternalIdByExternalId,
                        item.UnitExternalId);

                    if (unitId == null)
                        return RollbackUpdate(transaction, "Unidad no encontrada.", true);

                    await InsertItemAsync(connection, transaction, existing.Id, item, productId.Value, unitId.Value);
                }

                transaction.Commit();

                return new UpdateDeliveryResult
                {
                    Succeeded = true,
                    Message = "Entrega actualizada correctamente."
                };
            }
            catch
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
                    new { command.ExternalId },
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
                    new { DeliveryId = delivery.Id },
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
                            : (DateTime?)null
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
                        CreatedByUserId = delivery.SellerId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = delivery.Id
                    });

                transaction.Commit();

                return new ChangeDeliveryStatusResult
                {
                    Succeeded = true,
                    Message = "Estado de entrega actualizado correctamente."
                };
            }
            catch
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
                    new { ExternalId = command.DeliveryExternalId },
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
                            DeliveryId = delivery.Id
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
                            DeliveredQuantity = newDeliveredQuantity
                        },
                        transaction);

                    if (affectedRows == 0)
                        return RollbackConfirmReceipt(transaction, "No se pudo actualizar el item de la entrega.", false);

                    totalReceivedQuantity += item.ReceivedQuantity;
                }

                var quantities = (await connection.QueryAsync<DeliveryQuantityRow>(
                    DeliveryRepositoryQueries.GetQuantitiesByDeliveryId,
                    new { DeliveryId = delivery.Id },
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
                        DeliveredDateUtc = deliveredDateUtc
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
                        CreatedByUserId = delivery.SellerId,
                        RelatedEntityType = RelatedEntityType,
                        RelatedEntityId = delivery.Id,
                        MetadataJson = metadataJson
                    });

                transaction.Commit();

                return new ConfirmDeliveryReceiptResult
                {
                    Succeeded = true,
                    Message = "Recepcion de entrega registrada correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
                return new ConfirmDeliveryReceiptResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al registrar la recepcion de la entrega."
                };
            }
        }
        public async Task<DeleteDeliveryResult> DeleteAsync(string externalId)
        {
            using IDbConnection connection = CreateConnection();
            connection.Open();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                DeliveryInternalRow? delivery = await connection.QuerySingleOrDefaultAsync<DeliveryInternalRow>(
                    DeliveryRepositoryQueries.GetDeliveryInternalByExternalId,
                    new { ExternalId = externalId },
                    transaction);

                if (delivery == null)
                    return RollbackDelete(transaction, "Entrega no encontrada.", true);

                int affectedRows = await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.DeleteDelivery,
                    new { ExternalId = externalId },
                    transaction);

                if (affectedRows == 0)
                    return RollbackDelete(transaction, "Entrega no encontrada.", true);

                await connection.ExecuteAsync(
                    DeliveryRepositoryQueries.SoftDeleteItems,
                    new { DeliveryId = delivery.Id },
                    transaction);

                transaction.Commit();

                return new DeleteDeliveryResult
                {
                    Succeeded = true,
                    Message = "Entrega eliminada correctamente."
                };
            }
            catch
            {
                transaction.Rollback();
                return new DeleteDeliveryResult
                {
                    Succeeded = false,
                    Message = "Ocurrio un error al eliminar la entrega."
                };
            }
        }

        private static async Task<IReadOnlyList<DeliveryItemRow>> GetItemsAsync(
            IDbConnection connection,
            IReadOnlyCollection<int> deliveryIds)
        {
            if (deliveryIds.Count == 0)
                return new List<DeliveryItemRow>();

            var rows = await connection.QueryAsync<DeliveryItemRow>(
                DeliveryRepositoryQueries.GetItemsByDeliveryIds,
                new { DeliveryIds = deliveryIds });

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

        private static async Task<int?> GetInternalIdAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            string query,
            string externalId)
        {
            return await connection.QuerySingleOrDefaultAsync<int?>(
                query,
                new { ExternalId = externalId },
                transaction);
        }

        private static async Task InsertItemAsync(
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
                    item.DeliveredQuantity
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
    }
}
