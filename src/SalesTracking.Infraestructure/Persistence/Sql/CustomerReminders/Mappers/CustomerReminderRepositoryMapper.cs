using SalesTracking.Domain.Entities;
using SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders.Rows;

namespace SalesTracking.Infrastructure.Persistence.Sql.CustomerReminders.Mappers
{
    public static class CustomerReminderRepositoryMapper
    {
        public static CustomerReminder ToDomain(this CustomerReminderRow row)
        {
            return new CustomerReminder
            {
                Id = row.Id,
                ExternalId = row.ExternalId,
                Text = row.Text,
                Customer = new Customer() { Id = row.AssignedToId, ExternalId = row.AssignedToExternalId, Name = row.AssignedToName },
                Completed = row.Completed,
                ReminderAtUtc = row.ReminderAtUtc
            };
        }
    }
}
