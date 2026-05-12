using SalesTracking.Application.UseCases.CustomerReminders.Comands;
using SalesTracking.Application.UseCases.CustomerReminders.Results;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.CustomerReminders.Interfaces
{
    public interface ICustomerReminderService
    {
        Task<IReadOnlyList<CustomerReminderResult>> GetRemindersAsync(GetCustomerRemindersCommand command);
        Task<CreateCustomerReminderResult> CreateReminderAsync(CreateCustomerReminderCommand command);
        Task<CompleteCustomerReminderResult> CompleteReminderAsync(CompleteCustomerReminderCommand command);
    }
}
