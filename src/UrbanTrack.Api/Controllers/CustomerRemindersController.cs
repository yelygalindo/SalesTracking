using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.CustomerReminders.Comands;
using SalesTracking.Application.UseCases.CustomerReminders.Interfaces;
using SalesTracking.Application.UseCases.CustomerReminders.Results;
using UrbanTrack.Api.Controllers.Requests.CustomerReminders;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Customers;
using UrbanTrack.Api.Controllers.Responses.Mappers;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/customers/{customerExternalId}/reminders")]
    public class CustomerRemindersController : ControllerBase
    {
        private readonly ICustomerReminderService _service;
        private readonly ICurrentUser _currentUser;

        public CustomerRemindersController(ICustomerReminderService service, ICurrentUser currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerReminderResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerReminderResponse>>> GetReminders(
            string customerExternalId,
            [FromQuery] bool? completed = null)
        {
            IReadOnlyList<CustomerReminderResult> reminders = await _service.GetRemindersAsync(
                new GetCustomerRemindersCommand(customerExternalId, completed));

            return Ok(reminders.Select(x => x.ToResponse()));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IdMessageResponse>> CreateReminder(
            string customerExternalId,
            [FromBody] CustomerReminderRequest request)
        {
            CreateCustomerReminderResult result = await _service.CreateReminderAsync(
                request.ToApplication(customerExternalId, _currentUser.UserId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Created(string.Empty, result.ToResponse());
        }

        [HttpPatch("{reminderExternalId}/complete")]
        [ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageApiResponse>> CompleteReminder(
            string customerExternalId,
            string reminderExternalId)
        {
            CompleteCustomerReminderResult result = await _service.CompleteReminderAsync(
                new CompleteCustomerReminderCommand(
                    customerExternalId,
                    reminderExternalId,
                    _currentUser.UserId));

            if (!result.Succeeded)
                return NotFound(new ErrorResponse { Error = result.Message });

            return Ok(new MessageApiResponse { Message = result.Message });
        }
    }
}
