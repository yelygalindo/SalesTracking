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
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Update(string id, [FromBody] UpdateCustomerRequest req)
        {
            var exists = GetMockCustomers().Any(c => c.Id == 1);
            if (!exists) return await Task.FromResult(NotFound(new ErrorResponse { Error = "Cliente no encontrado." }));
            return await Task.FromResult(Ok(new MessageResponse { Message = "Cliente actualizado (mock)." }));
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MessageApiResponse>> ChangeStatus(string id, [FromBody] ChangeStatusRequest req)
        {
            return await Task.FromResult(Ok(new MessageApiResponse { Message = $"Estado cambiado a '{req.Status}' (mock)." }));
        }       

        // Helpers - mocked data
        private List<CustomerSummaryResponse> GetMockCustomers()
        {
            return new List<CustomerSummaryResponse>
            {
                new CustomerSummaryResponse { Name = "Construcciones Rivera", CompanyName = "Rivera S.A.", Phone = "+52 55 1234 5678", Email = "contacto@rivera.mx", Status = "active",  CreatedAt = DateTime.UtcNow.AddMonths(-6) },
                new CustomerSummaryResponse { Name = "Almacen Obra S.A.", CompanyName = "AlmacenObra", Phone = "+52 55 8765 4321", Email = "ventas@almacenobra.mx", Status = "prospect", CreatedAt = DateTime.UtcNow.AddMonths(-1) },
                new CustomerSummaryResponse { Name = "Grupo Aislantes", CompanyName = "Aislantes Norte", Phone = "+52 55 2222 3333", Email = "info@aislantes.mx", Status = "active", CreatedAt = DateTime.UtcNow.AddYears(-1) }
            };
        }
    }
}