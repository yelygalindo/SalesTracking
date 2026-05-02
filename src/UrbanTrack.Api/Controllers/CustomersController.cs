using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanTrack.Api.Controllers.Requests.Customers;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Customers;
using UrbanTrack.Api.Controllers.Responses.Pagination;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<CustomerSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<CustomerSummaryResponse>>> Get(
            [FromQuery] string? status,
            [FromQuery] string? sellerId,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var all = GetMockCustomers();

            var filtered = all.Where(c =>
                (string.IsNullOrEmpty(status) || c.Status.Equals(status, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(sellerId) || c.SellerId == sellerId) &&
                (string.IsNullOrEmpty(search) || c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || c.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            var pagedItems = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var response = new PagedResponse<CustomerSummaryResponse>
            {
                Items = pagedItems,
                Pagination = new PaginationResponse
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = filtered.Count,
                    TotalPages = (int)Math.Ceiling(filtered.Count / (double)pageSize)
                }
            };

            return await Task.FromResult(Ok(response));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDetailResponse>> GetById(string id)
        {
            var customer = GetMockCustomers().FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return await Task.FromResult(NotFound(new ErrorResponse { Error = "Cliente no encontrado." }));
            }

            var detail = new CustomerDetailResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                CompanyName = customer.CompanyName,
                Phone = customer.Phone,
                Email = customer.Email,
                Status = customer.Status,
                SellerId = customer.SellerId,
                Address = "Av. Principal 123",
                Latitude = 19.4326m,
                Longitude = -99.1332m,
                CreatedAt = customer.CreatedAt,
                Notes = new List<CustomerNoteResponse>
                {
                    new CustomerNoteResponse { Id = "n-1", Text = "Cliente interesado en Calaminas galvanizadas.", AuthorId = "u-2", CreatedAt = DateTime.UtcNow.AddDays(-3) }
                }
            };

            return await Task.FromResult(Ok(detail));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdMessageResponse>> Create([FromBody] CreateCustomerRequest req)
        {
            var id = $"c-{Guid.NewGuid():N}".Substring(0, 8);
            var response = new IdMessageResponse { Id = id, Message = "Cliente creado (mock)." };
            return await Task.FromResult(CreatedAtAction(nameof(GetById), new { id = response.Id }, response));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Update(string id, [FromBody] UpdateCustomerRequest req)
        {
            var exists = GetMockCustomers().Any(c => c.Id == id);
            if (!exists) return await Task.FromResult(NotFound(new ErrorResponse { Error = "Cliente no encontrado." }));
            return await Task.FromResult(Ok(new MessageResponse { Message = "Cliente actualizado (mock)." }));
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MessageResponse>> ChangeStatus(string id, [FromBody] ChangeStatusRequest req)
        {
            return await Task.FromResult(Ok(new MessageResponse { Message = $"Estado cambiado a '{req.Status}' (mock)." }));
        }

        [HttpGet("{id}/notes")]
        [ProducesResponseType(typeof(IEnumerable<CustomerNoteResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerNoteResponse>>> GetNotes(string id)
        {
            var notes = new List<CustomerNoteResponse>
            {
                new CustomerNoteResponse { Id = "n-1", Text = "Cotizar 200 m2 de Aislantes.", AuthorId = "u-1", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new CustomerNoteResponse { Id = "n-2", Text = "Confirmar entrega de clavijas.", AuthorId = "u-3", CreatedAt = DateTime.UtcNow.AddDays(-1) }
            };

            return await Task.FromResult(Ok(notes));
        }

        [HttpPost("{id}/notes")]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<IdMessageResponse>> AddNote(string id, [FromBody] CustomerNoteRequest req)
        {
            var noteId = $"n-{Guid.NewGuid():N}".Substring(0, 8);
            return await Task.FromResult(Created(string.Empty, new IdMessageResponse { Id = noteId, Message = "Nota agregada (mock)." }));
        }

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
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MessageResponse>> CompleteReminder(string id, string reminderId)
        {
            return await Task.FromResult(Ok(new MessageResponse { Message = "Recordatorio marcado como completado (mock)." }));
        }

        [HttpGet("{id}/timeline")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetTimeline(string id)
        {
            var timeline = new List<string>
            {
                "2026-04-01: Cotización enviada.",
                "2026-04-05: Llamada de seguimiento.",
                "2026-04-10: Cliente convertido a activo."
            };
            return await Task.FromResult(Ok(timeline));
        }

        // Helpers - mocked data
        private List<CustomerSummaryResponse> GetMockCustomers()
        {
            return new List<CustomerSummaryResponse>
            {
                new CustomerSummaryResponse { Id = "c-101", Name = "Construcciones Rivera", CompanyName = "Rivera S.A.", Phone = "+52 55 1234 5678", Email = "contacto@rivera.mx", Status = "active", SellerId = "u-1", CreatedAt = DateTime.UtcNow.AddMonths(-6) },
                new CustomerSummaryResponse { Id = "c-205", Name = "Almacen Obra S.A.", CompanyName = "AlmacenObra", Phone = "+52 55 8765 4321", Email = "ventas@almacenobra.mx", Status = "prospect", SellerId = "u-2", CreatedAt = DateTime.UtcNow.AddMonths(-1) },
                new CustomerSummaryResponse { Id = "c-333", Name = "Grupo Aislantes", CompanyName = "Aislantes Norte", Phone = "+52 55 2222 3333", Email = "info@aislantes.mx", Status = "active", SellerId = "u-3", CreatedAt = DateTime.UtcNow.AddYears(-1) }
            };
        }
    }
}