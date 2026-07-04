using SalesTracking.Application.UseCases.Customers.Comands;
using SalesTracking.Application.UseCases.Customers.Results;

namespace SalesTracking.Application.UseCases.Customers.Interfaces
{
    public interface ICustomerService
    {
        Task<GetCustomersResult> GetCustomersAsync(GetCustomersCommand command);
        Task<CustomerDetailResult?> GetCustomerByIdAsync(GetCustomerByIdCommand command);
        Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerCommand command);
        Task<IReadOnlyList<CustomerStatusResult>> GetCustomerStatusesAsync();
        Task<UpdateCustomerResult> UpdateCustomerAsync(UpdateCustomerCommand command);
        Task<ChangeCustomerStatusResult> ChangeCustomerStatusAsync(ChangeCustomerStatusCommand command);
        Task<DeleteCustomerResult> DeleteCustomerAsync(DeleteCustomerCommand command);
    }
}
