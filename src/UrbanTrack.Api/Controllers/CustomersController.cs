using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Customers.Comands;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Customers.Interfaces;
using SalesTracking.Application.UseCases.Customers.Results;
using UrbanTrack.Api.Controllers.Requests.Customers;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Customers;
using UrbanTrack.Api.Controllers.Responses.Mappers;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;
        private readonly ICurrentUser _currentUser;

        public CustomersController(ICustomerService service, ICurrentUser currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [HttpGet("statuses")]
        [ProducesResponseType(typeof(IEnumerable<CustomerStatusResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerStatusResponse>>> GetStatuses()
        {
            IReadOnlyList<CustomerStatusResult> statuses = await _service.GetCustomerStatusesAsync();

            return Ok(statuses.Select(x => x.ToResponse()));
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetCustomersResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetCustomersResponse>> Get([FromQuery] GetCustomersRequest getCustomersRequest)
        {
            GetCustomersResult result = await _service.GetCustomersAsync(getCustomersRequest.ToApplication());
            return Ok(result.ToResponse());
        }

        [HttpGet("{externalId}")]
        [ProducesResponseType(typeof(CustomerDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDetailResponse>> GetById(string externalId)
        {
            CustomerDetailResult? customer = await _service.GetCustomerByIdAsync(new GetCustomerByIdCommand(externalId));
            if (customer == null)
                return NotFound(new ErrorResponse { Error = "Cliente no encontrado." });

            return Ok(customer.ToResponse());
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdMessageResponse>> Create([FromBody] CreateCustomerRequest createCustomer)
        {
            CreateCustomerResult createCustomerResult = await _service.CreateCustomerAsync(
                createCustomer.ToApplication(_currentUser.UserId.GetValueOrDefault()));

            if (!createCustomerResult.Succeeded)
                return BadRequest(new ErrorResponse { Error = createCustomerResult.Message });

            return CreatedAtAction(
                nameof(GetById),
                new { externalId = createCustomerResult.Id },
                createCustomerResult.ToResponse());
        }

        [HttpPut("{externalId}")]
        [ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageApiResponse>> Update(string externalId,[FromBody] UpdateCustomerRequest request)
        {
            UpdateCustomerResult result = await _service.UpdateCustomerAsync(
                request.ToApplication(externalId, _currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageApiResponse { Message = result.Message });
        }

        [HttpPatch("{externalId}/status")]
        [ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageApiResponse>> ChangeStatus(string externalId,[FromBody] ChangeStatusRequest request)
        {
            ChangeCustomerStatusResult result = await _service.ChangeCustomerStatusAsync(
                request.ToApplication(externalId, _currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageApiResponse { Message = result.Message });
        }

        [HttpDelete("{externalId}")]
        [ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageApiResponse>> Delete(string externalId)
        {
            DeleteCustomerResult result = await _service.DeleteCustomerAsync(
                new DeleteCustomerCommand(externalId, _currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageApiResponse { Message = result.Message });
        }
    }
}
