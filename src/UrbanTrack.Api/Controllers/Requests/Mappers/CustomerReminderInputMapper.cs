using SalesTracking.Application.UseCases.CustomerReminders.Comands;
using UrbanTrack.Api.Controllers.Requests.CustomerReminders;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class CustomerReminderInputMapper
    {
        public static CreateCustomerReminderCommand ToApplication(
            this CustomerReminderRequest request,
            string customerId,
            int createdByUserId)
        {
            return new CreateCustomerReminderCommand
            {
                CustomerExternalId = customerId,
                Text = request.Text,
                ReminderAtUtc = request.ReminderAtUtc,
                AssignedToId = request.AssignedToId,
                CreatedByUserId = createdByUserId
            };
        }
    }
}