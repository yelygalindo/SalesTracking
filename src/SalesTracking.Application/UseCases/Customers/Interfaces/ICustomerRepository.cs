using SalesTracking.Application.UseCases.Customers.Models;
using SalesTracking.Domain.Entities;
using SalesTracking.Domain.Enums;

namespace SalesTracking.Application.UseCases.Customers.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerPagedList> GetCustomersAsync(GetCustomersFilter getCustomersFilter);
        Task<Customer?> GetCustomerByExternalIdAsync(string externalId);
        Task<CreateCustomer> CreateCustomerAsync(CreateCustomer customer);
        Task<UpdateCustomer> UpdateCustomerAsync(UpdateCustomer customer);
        Task<bool> ChangeCustomerStatusAsync(string externalId, CustomerStatus status);
    }
}
