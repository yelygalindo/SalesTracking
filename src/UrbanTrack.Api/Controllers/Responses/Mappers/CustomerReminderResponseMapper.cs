using SalesTracking.Application.UseCases.CustomerReminders.Results;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.CustomerReminders;

namespace UrbanTrack.Api.Controllers.Responses.Mappers
{
    public static class CustomerReminderResponseMapper
    {
        public static CustomerReminderResponse ToResponse(this
            CustomerReminderResult reminder)
        {
            return new CustomerReminderResponse
            {
                Id = reminder.Id,
                ExternalId = reminder.ExternalId,
                Text = reminder.Text,
                ReminderAt = reminder.ReminderAtUtc,
                AssignedTo = new AssignedReminderResponse(reminder.Customer.Id, reminder.Customer.ExternalId, reminder.Customer.Name),
                Completed = reminder.Completed
            };
        }

        public static IdMessageResponse ToResponse(this CreateCustomerReminderResult result)
        {
            return new IdMessageResponse
            {
                Id = result.Id,
                Message = result.Message
            };
        }
    }
}
