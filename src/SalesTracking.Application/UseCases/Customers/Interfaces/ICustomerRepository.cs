using SalesTracking.Application.UseCases.Customers.Models;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.Customers.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerPagedList> GetCustomersAsync(GetCustomersFilter getCustomersFilter);
        Task<Customer?> GetCustomerByExternalIdAsync(string externalId);
        Task<CreateCustomer> CreateCustomerAsync(CreateCustomer customer);
    }
}
