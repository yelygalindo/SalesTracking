using SalesTracking.Application.UseCases.CustomerReminders.Models;
using SalesTracking.Domain.Entities;

namespace SalesTracking.Application.UseCases.CustomerReminders.Interfaces
{
    public interface ICustomerReminderRepository
    {
        Task<IReadOnlyList<CustomerReminder>> GetRemindersAsync(string customerExternalId);
        Task<CreateCustomerReminder> CreateReminderAsync(CreateCustomerReminder reminder);
        Task<bool> CompleteReminderAsync(
            string customerExternalId,
            string reminderExternalId,
            int completedByUserId);
    }
}
