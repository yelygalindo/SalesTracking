using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.Common.ExternalIds;
using SalesTracking.Application.UseCases.Customers.Interfaces;
using SalesTracking.Application.UseCases.Customers.Mappers;
using SalesTracking.Application.UseCases.Customers.Models;
using SalesTracking.Domain.Entities;
using SalesTracking.Domain.Enums;
using SalesTracking.Infrastructure.Persistence.Settings;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerNotes.Rows;
using SalesTracking.Infrastructure.Persistence.Sql.Customers.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.Customers.Rows;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders.Mappers;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders.Rows;
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseSettings _databaseOptions;
        private readonly ICurrentUser _currentUser;

        public CustomerRepository(IOptions<DatabaseSettings> databaseOptions, ICurrentUser currentUser)
        {
            _databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
            _currentUser = currentUser;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_databaseOptions.ConnectionString);
        private int CompanyId => _currentUser.CompanyId;
        private bool IsSeller => _currentUser.Roles.Contains("seller", StringComparer.OrdinalIgnoreCase);

        public async Task<Customer?> GetCustomerByExternalIdAsync(string externalId)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                return null;

            using var conn = CreateConnection();

            using SqlMapper.GridReader results = await conn.QueryMultipleAsync(
                CustomerRepositoryQueries.GetCustomerDetail,
                new { ExternalId = externalId, CompanyId });

            CustomerDetailRow? customerDetailRow =
                await results.ReadFirstOrDefaultAsync<CustomerDetailRow>();

            if (customerDetailRow == null)
                return null;

            IEnumerable<CustomerNoteRow> notes = await results.ReadAsync<CustomerNoteRow>();

            Customer customer = customerDetailRow.ToDomain();
            customer.Notes = notes.Select(x => x.ToDomain()).ToList();

            IEnumerable<CustomerReminderRow> reminders = await results.ReadAsync<CustomerReminderRow>();

            customer.Reminders = reminders.Select(x => x.ToDomain()).ToList();

            return customer;        
        }

        public async Task<CustomerPagedList> GetCustomersAsync(GetCustomersFilter filter)
        {
            using var conn = CreateConnection();

            int page = filter.Page <= 0 ? 1 : filter.Page;
            int pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;
            int offset = (page - 1) * pageSize;

            var parameters = new
            {
                StatusId = filter.Status,
                SellerExternalId = IsSeller ? _currentUser.UserExternalId : filter.SellerExternalId,
                Search = string.IsNullOrWhiteSpace(filter.Search)
                    ? null
                    : $"%{filter.Search.Trim()}%",
                Offset = offset,
                PageSize = pageSize,
                CompanyId
            };

            using SqlMapper.GridReader results = await conn.QueryMultipleAsync(
                CustomerRepositoryQueries.GetCustomersPage,
                parameters);

            IEnumerable<CustomerSummaryRow> items = await results.ReadAsync<CustomerSummaryRow>();
            int totalItems = await results.ReadSingleAsync<int>();

            return new CustomerPagedList
            {
                Items = items.Select(x => x.ToDomain()).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task<CreateCustomer> CreateCustomerAsync(CreateCustomer customer)
        {
            using var conn = CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                int? sellerInternalId = null;

                string? assignedSeller = customer.SellerExternalId;
                if (!string.IsNullOrWhiteSpace(assignedSeller))
                {
                    sellerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                        CustomerRepositoryQueries.GetSellerInternalIdByExternalId,
                        new { ExternalId = assignedSeller, CompanyId },
                        transaction);

                    if (sellerInternalId == null)
                    {
                        transaction.Rollback();

                        customer.Succeeded = false;
                        customer.Message = "El vendedor asignado no existe.";

                        return customer;
                    }
                }

                int customerId = await conn.ExecuteScalarAsync<int>(
                    CustomerRepositoryQueries.CreateCustomer,
                    new
                    {
                        customer.ExternalId,
                        customer.Name,
                        customer.CompanyName,
                        customer.Phone,
                        customer.Email,
                        StatusId = (int)customer.Status,
                        SellerId = sellerInternalId,
                        customer.Address,
                        customer.Latitude,
                        customer.Longitude,
                        CompanyId
                    },
                    transaction);

                await conn.ExecuteAsync(
                    CustomerRepositoryQueries.CreateCustomerTimelineEvent,
                    new
                    {
                        ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerTimelineEvent),
                        CustomerId = customerId,
                        EventType = "CustomerCreated",
                        Description = "Cliente creado.",
                        CreatedById = customer.CreatedByUserId,
                        CompanyId
                    },
                    transaction);

                transaction.Commit();

                customer.Succeeded = true;
                customer.Message = "Cliente creado correctamente.";

                return customer;
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();

                customer.Succeeded = false;
                customer.Message = "Ocurrió un error al crear el cliente.";

                return customer;
            }
        }

        public async Task<UpdateCustomer> UpdateCustomerAsync(UpdateCustomer customer)
        {
            using var conn = CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                int? customerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                    CustomerRepositoryQueries.GetCustomerInternalIdByExternalId,
                    new { customer.ExternalId, CompanyId },
                    transaction);

                if (customerInternalId == null)
                {
                    transaction.Rollback();

                    customer.Succeeded = false;
                    customer.NotFound = true;
                    customer.Message = "Cliente no encontrado.";

                    return customer;
                }

                int? sellerInternalId = null;

                string? assignedSeller = customer.SellerId;
                if (!string.IsNullOrWhiteSpace(assignedSeller))
                {
                    sellerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                        CustomerRepositoryQueries.GetSellerInternalIdByExternalId,
                        new { ExternalId = assignedSeller, CompanyId },
                        transaction);

                    if (sellerInternalId == null)
                    {
                        transaction.Rollback();

                        customer.Succeeded = false;
                        customer.Message = "El vendedor asignado no existe.";

                        return customer;
                    }
                }

                await conn.ExecuteAsync(
                    CustomerRepositoryQueries.UpdateCustomer,
                    new
                    {
                        CustomerId = customerInternalId.Value,
                        customer.Name,
                        customer.CompanyName,
                        customer.Phone,
                        customer.Email,
                        SellerId = sellerInternalId,
                        customer.Address,
                        customer.Latitude,
                        customer.Longitude,
                        CompanyId
                    },
                    transaction);

                await conn.ExecuteAsync(
                    CustomerRepositoryQueries.CreateCustomerTimelineEvent,
                    new
                    {
                        ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerTimelineEvent),
                        CustomerId = customerInternalId.Value,
                        EventType = "CustomerUpdated",
                        Description = "Cliente actualizado.",
                        CreatedById = customer.UpdatedByUserId,
                        CompanyId
                    },
                    transaction);

                transaction.Commit();

                customer.Succeeded = true;
                customer.Message = "Cliente actualizado correctamente.";

                return customer;
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();

                customer.Succeeded = false;
                customer.Message = "Ocurrió un error al actualizar el cliente.";

                return customer;
            }
        }

        public async Task<bool> ChangeCustomerStatusAsync(string externalId, CustomerStatus status, int changedByUserId)
        {
            using var conn = CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                int? customerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                    CustomerRepositoryQueries.GetCustomerInternalIdByExternalId,
                    new { ExternalId = externalId, CompanyId },
                    transaction);

                if (customerInternalId == null)
                {
                    transaction.Rollback();
                    return false;
                }

                await conn.ExecuteAsync(
                    CustomerRepositoryQueries.ChangeCustomerStatus,
                    new
                    {
                        CustomerId = customerInternalId.Value,
                        StatusId = (int)status,
                        CompanyId
                    },
                    transaction);

                await conn.ExecuteAsync(
                    CustomerRepositoryQueries.CreateCustomerTimelineEvent,
                    new
                    {
                        ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerTimelineEvent),
                        CustomerId = customerInternalId.Value,
                        EventType = "CustomerStatusChanged",
                        Description = $"Estado cambiado a '{status.ToLabel()}'.",
                        CreatedById = changedByUserId,
                        CompanyId
                    },
                    transaction);

                transaction.Commit();

                return true;
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> DeleteCustomerAsync(string externalId, int deletedByUserId)
        {
            using var conn = CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                int? customerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                    CustomerRepositoryQueries.GetCustomerInternalIdByExternalId,
                    new { ExternalId = externalId, CompanyId },
                    transaction);

                if (customerInternalId == null)
                {
                    transaction.Rollback();
                    return false;
                }

                await conn.ExecuteAsync(
                    CustomerRepositoryQueries.DeleteCustomer,
                    new { CustomerId = customerInternalId.Value, CompanyId },
                    transaction);

                await conn.ExecuteAsync(
                    CustomerRepositoryQueries.CreateCustomerTimelineEvent,
                    new
                    {
                        ExternalId = ExternalIdGenerator.New(ExternalIdPrefixes.CustomerTimelineEvent),
                        CustomerId = customerInternalId.Value,
                        EventType = "CustomerDeleted",
                        Description = "Cliente eliminado.",
                        CreatedById = deletedByUserId,
                        CompanyId
                    },
                    transaction);

                transaction.Commit();

                return true;
            }
            catch (Exception exception) when (SalesTracking.Infrastructure.Logging.InfrastructureExceptionLogger.Log(exception))
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
