using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Customers.Comands;
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
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
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
            CreateCustomerResult createCustomerResult = await _service.CreateCustomerAsync(createCustomer.ToApplication());

            if (!createCustomerResult.Succeeded)
                return BadRequest(new ErrorResponse { Error = createCustomerResult.Message });

            return CreatedAtAction(
                nameof(GetById),
                new { id = createCustomerResult.Id },
                createCustomerResult.ToResponse());
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageApiResponse>> Update(int id,[FromBody] UpdateCustomerRequest request)
        {
            UpdateCustomerResult result = await _service.UpdateCustomerAsync(
                request.ToApplication(id));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageApiResponse { Message = result.Message });
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageApiResponse>> ChangeStatus(int id,[FromBody] ChangeStatusRequest request)
        {
            ChangeCustomerStatusResult result = await _service.ChangeCustomerStatusAsync(request.ToApplication(id));

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