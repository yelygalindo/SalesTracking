using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
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
using System.Data;

namespace SalesTracking.Infrastructure.Persistence.Sql.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseSettings _databaseOptions;

        public CustomerRepository(IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
        }

        private IDbConnection CreateConnection() => new SqlConnection(_databaseOptions.ConnectionString);

        public async Task<Customer?> GetCustomerByExternalIdAsync(string externalId)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                return null;

            using var conn = CreateConnection();

            CustomerDetailRow? customerDetailRow =
                await conn.QueryFirstOrDefaultAsync<CustomerDetailRow>(
                    CustomerRepositoryQueries.GetCustomerByExternalId,
                    new { ExternalId = externalId });

            if (customerDetailRow == null)
                return null;

            IEnumerable<CustomerNoteRow> notes =
                await conn.QueryAsync<CustomerNoteRow>(
                    CustomerRepositoryQueries.GetCustomerNotesByCustomerId,
                    new { CustomerId = customerDetailRow.Id });

            Customer customer = customerDetailRow.ToDomain();
            customer.Notes = notes.Select(x => x.ToDomain()).ToList();

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
                SellerExternalId = filter.SellerExternalId,
                Search = string.IsNullOrWhiteSpace(filter.Search)
                    ? null
                    : $"%{filter.Search.Trim()}%",
                Offset = offset,
                PageSize = pageSize
            };

            IEnumerable<CustomerSummaryRow> items =
                await conn.QueryAsync<CustomerSummaryRow>(
                    CustomerRepositoryQueries.GetCustomers,
                    parameters);

            int totalItems =
                await conn.ExecuteScalarAsync<int>(
                    CustomerRepositoryQueries.CountCustomers,
                    parameters);

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

                if (!string.IsNullOrWhiteSpace(customer.RegisterByExternalId))
                {
                    sellerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                        CustomerRepositoryQueries.GetSellerInternalIdByExternalId,
                        new { ExternalId = customer.RegisterByExternalId },
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
                        customer.Longitude
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
                        CreatedById = sellerInternalId
                    },
                    transaction);

                transaction.Commit();

                customer.Succeeded = true;
                customer.Message = "Cliente creado correctamente.";

                return customer;
            }
            catch
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
                    new { customer.ExternalId },
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

                if (!string.IsNullOrWhiteSpace(customer.SellerId))
                {
                    sellerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                        CustomerRepositoryQueries.GetSellerInternalIdByExternalId,
                        new { ExternalId = customer.SellerId },
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
                        customer.Longitude
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
                        CreatedById = sellerInternalId
                    },
                    transaction);

                transaction.Commit();

                customer.Succeeded = true;
                customer.Message = "Cliente actualizado correctamente.";

                return customer;
            }
            catch
            {
                transaction.Rollback();

                customer.Succeeded = false;
                customer.Message = "Ocurrió un error al actualizar el cliente.";

                return customer;
            }
        }

        public async Task<bool> ChangeCustomerStatusAsync(string externalId, CustomerStatus status)
        {
            using var conn = CreateConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                int? customerInternalId = await conn.QueryFirstOrDefaultAsync<int?>(
                    CustomerRepositoryQueries.GetCustomerInternalIdByExternalId,
                    new { ExternalId = externalId },
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
                        StatusId = (int)status
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
                        CreatedById = (int?)null
                    },
                    transaction);

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
