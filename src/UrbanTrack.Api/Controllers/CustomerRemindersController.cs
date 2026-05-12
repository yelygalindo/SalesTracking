using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanTrack.Api.Controllers.Requests.CustomerReminders;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Customers;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerRemindersController : ControllerBase
    {
        [HttpGet("{id}/reminders")]
        [ProducesResponseType(typeof(IEnumerable<CustomerReminderResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerReminderResponse>>> GetReminders(string id)
        {
            var reminders = new List<CustomerReminderResponse>
            {
                new CustomerReminderResponse { Id = "r-1", Text = "Visitar obra", ReminderAt = DateTime.UtcNow.AddDays(2), AssignedToId = "u-2", Completed = false }
            };
            return await Task.FromResult(Ok(reminders));
        }

        [HttpPost("{id}/reminders")]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<IdMessageResponse>> CreateReminder(string id, [FromBody] CustomerReminderRequest req)
        {
            var rid = $"r-{Guid.NewGuid():N}".Substring(0, 8);
            return await Task.FromResult(Created(string.Empty, new IdMessageResponse { Id = rid, Message = "Recordatorio creado (mock)." }));
        }

        [HttpPatch("{id}/reminders/{reminderId}/complete")]
        [ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MessageApiResponse>> CompleteReminder(string id, string reminderId)
        {
            return await Task.FromResult(Ok(new MessageApiResponse { Message = "Recordatorio marcado como completado (mock)." }));
        }
    }
}
