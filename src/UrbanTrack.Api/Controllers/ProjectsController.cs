using Microsoft.AspNetCore.Mvc;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Projects;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using Microsoft.AspNetCore.Http;
using UrbanTrack.Api.Controllers.Responses.Products;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProductSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ProductSummaryResponse>>> Get(
            [FromQuery] string? status,
            [FromQuery] string? customerId,
            [FromQuery] string? sellerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var all = GetMockProjects();

            var filtered = all.Where(p =>
                (string.IsNullOrEmpty(status) || p.Status.Equals(status, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(customerId) || p.CustomerId == customerId) &&
                (string.IsNullOrEmpty(sellerId) || p.SellerId == sellerId)
            ).ToList();

            var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var response = new PagedResponse<ProductSummaryResponse>
            {
                Items = items,
                Pagination = new PaginationResponse { Page = page, PageSize = pageSize, TotalItems = filtered.Count, TotalPages = (int)Math.Ceiling(filtered.Count / (double)pageSize) }
            };

            return await Task.FromResult(Ok(response));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDetailResponse>> GetById(string id)
        {
            var project = GetMockProjects().FirstOrDefault(p => p.Id == id);
            if (project == null) return await Task.FromResult(NotFound(new ErrorResponse { Error = "Proyecto no encontrado." }));

            var detail = new ProjectDetailResponse
            {
                Id = project.Id,
                Name = project.Name,
                CustomerId = project.CustomerId,
                SellerId = project.SellerId,
                Status = project.Status,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddMonths(2),
                Products = new List<ProjectProductResponse>
                {
                    new ProjectProductResponse { ProductId = "prod-01", Name = "Calamina galvanizada", Quantity = 120.5m, Unit = "m2" }
                }
            };

            return await Task.FromResult(Ok(detail));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<IdMessageResponse>> Create([FromBody] CreateProjectRequest req)
        {
            var id = $"p-{Guid.NewGuid():N}".Substring(0,8);
            return await Task.FromResult(Created(string.Empty, new IdMessageResponse { Id = id, Message = "Proyecto creado (mock)." }));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MessageResponse>> Update(string id, [FromBody] CreateProjectRequest req)
        {
            return await Task.FromResult(Ok(new MessageResponse { Message = "Proyecto actualizado (mock)." }));
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MessageResponse>> ChangeStatus(string id, [FromBody] dynamic body)
        {
            return await Task.FromResult(Ok(new MessageResponse { Message = "Estado de proyecto actualizado (mock)." }));
        }

        [HttpGet("{id}/products")]
        [ProducesResponseType(typeof(IEnumerable<ProjectProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProjectProductResponse>>> GetProducts(string id)
        {
            var products = new List<ProjectProductResponse>
            {
                new ProjectProductResponse { ProductId = "prod-01", Name = "Aislante termo", Quantity = 50m, Unit = "m2" }
            };
            return await Task.FromResult(Ok(products));
        }

        [HttpPost("{id}/products")]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<IdMessageResponse>> AddProduct(string id, [FromBody] ProjectProductResponse req)
        {
            var pid = $"pp-{Guid.NewGuid():N}".Substring(0,8);
            return await Task.FromResult(Created(string.Empty, new IdMessageResponse { Id = pid, Message = "Producto agregado al proyecto (mock)." }));
        }

        [HttpPut("{id}/products/{productId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MessageResponse>> UpdateProduct(string id, string productId, [FromBody] ProjectProductResponse req)
        {
            return await Task.FromResult(Ok(new MessageResponse { Message = "Producto actualizado (mock)." }));
        }

        [HttpDelete("{id}/products/{productId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MessageResponse>> DeleteProduct(string id, string productId)
        {
            return await Task.FromResult(Ok(new MessageResponse { Message = "Producto eliminado del proyecto (mock)." }));
        }

        [HttpGet("{id}/attachments")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetAttachments(string id)
        {
            return await Task.FromResult(Ok(new List<string> { "plano-obra.pdf", "fotografias-iniciales.zip" }));
        }

        [HttpPost("{id}/attachments")]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<IdMessageResponse>> UploadAttachment(string id)
        {
            var aid = $"a-{Guid.NewGuid():N}".Substring(0,8);
            return await Task.FromResult(Created(string.Empty, new IdMessageResponse { Id = aid, Message = "Adjunto subido (mock)." }));
        }

        [HttpGet("{id}/timeline")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetTimeline(string id)
        {
            var timeline = new List<string> { "2026-03-01: Proyecto creado", "2026-04-01: Reunión comercial" };
            return await Task.FromResult(Ok(timeline));
        }

        private List<ProductSummaryResponse> GetMockProjects()
        {
            return new List<ProductSummaryResponse>
            {
                new ProductSummaryResponse { Id = "p-11", Name = "Centro Comercial Las Palmas", CustomerId = "c-101", SellerId = "u-1", Status = "in_progress", CreatedAt = DateTime.UtcNow.AddMonths(-2) },
                new ProductSummaryResponse { Id = "p-22", Name = "Vivienda Social Alto Verde", CustomerId = "c-205", SellerId = "u-2", Status = "planning", CreatedAt = DateTime.UtcNow.AddMonths(-1) }
            };
        }
    }
}