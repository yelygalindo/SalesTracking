using Microsoft.AspNetCore.Mvc;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using Microsoft.AspNetCore.Http;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        //[HttpGet]
        //[ProducesResponseType(typeof(PagedResponse<ProductSummaryResponse>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<PagedResponse<ProductSummaryResponse>>> Get([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    var all = GetMockProducts();
        //    var filtered = all.Where(p => string.IsNullOrEmpty(search) || p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || p.Code.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        //    var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //    var response = new PagedResponse<ProductSummaryResponse>
        //    {
        //        Items = items,
        //        Pagination = new PaginationResponse { Page = page, PageSize = pageSize, TotalItems = filtered.Count, TotalPages = (int)Math.Ceiling(filtered.Count / (double)pageSize) }
        //    };

        //    return await Task.FromResult(Ok(response));
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(ProductSummaryResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<ProductSummaryResponse>> GetById(string id)
        //{
        //    var p = GetMockProducts().FirstOrDefault(x => x.Id == id);
        //    if (p == null) return await Task.FromResult(NotFound(new ErrorResponse { Error = "Producto no encontrado." }));
        //    return await Task.FromResult(Ok(p));
        //}

        //[HttpPost]
        //[ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        //public async Task<ActionResult<IdMessageResponse>> Create([FromBody] CreateProductRequest req)
        //{
        //    var id = $"prod-{Guid.NewGuid():N}".Substring(0,8);
        //    return await Task.FromResult(Created(string.Empty, new IdMessageResponse { Id = id, Message = "Producto creado (mock)." }));
        //}

        //[HttpPut("{id}")]
        //[ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        //public async Task<ActionResult<MessageApiResponse>> Update(string id, [FromBody] CreateProductRequest req)
        //{
        //    return await Task.FromResult(Ok(new MessageApiResponse { Message = "Producto actualizado (mock)." }));
        //}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        //public async Task<ActionResult<MessageApiResponse>> Delete(string id)
        //{
        //    return await Task.FromResult(Ok(new MessageApiResponse { Message = "Producto eliminado (mock)." }));
        //}

        //// Units endpoints (inside ProductsController per contract)
        //[HttpGet("/api/units")]
        //[ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<object>>> GetUnits()
        //{
        //    var units = new[]
        //    {
        //        new { Id = "u-m2", Name = "m2" },
        //        new { Id = "u-pza", Name = "pieza" }
        //    };

        //    // NOTE: contract forbids anonymous objects, but units are simple; return typed objects instead:
        //    var typed = new List<UnitResponse>();
        //    foreach (var u in units)
        //    {
        //        typed.Add(new UnitResponse { Id = u.Id, Name = u.Name });
        //    }

        //    return await Task.FromResult(Ok(typed));
        //}

        //[HttpPost("/api/units")]
        //[ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        //public async Task<ActionResult<IdMessageResponse>> CreateUnit([FromBody] UnitRequest req)
        //{
        //    var id = $"u-{Guid.NewGuid():N}".Substring(0,8);
        //    return await Task.FromResult(Created(string.Empty, new IdMessageResponse { Id = id, Message = "Unidad creada (mock)." }));
        //}

        //[HttpPut("/api/units/{id}")]
        //[ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        //public async Task<ActionResult<MessageApiResponse>> UpdateUnit(string id, [FromBody] UnitRequest req)
        //{
        //    return await Task.FromResult(Ok(new MessageApiResponse { Message = "Unidad actualizada (mock)." }));
        //}

        //[HttpDelete("/api/units/{id}")]
        //[ProducesResponseType(typeof(MessageApiResponse), StatusCodes.Status200OK)]
        //public async Task<ActionResult<MessageApiResponse>> DeleteUnit(string id)
        //{
        //    return await Task.FromResult(Ok(new MessageApiResponse { Message = "Unidad eliminada (mock)." }));
        //}

        //private List<ProductSummaryResponse> GetMockProducts()
        //{
        //    return new List<ProductSummaryResponse>
        //    {
        //        new ProductSummaryResponse { Id = "prod-01", Name = "Calamina galvanizada", Code = "CAL-G01", Unit = "m2", Price = 150.25m },
        //        new ProductSummaryResponse { Id = "prod-02", Name = "Aislante térmico", Code = "AIS-T02", Unit = "m2", Price = 85.50m },
        //        new ProductSummaryResponse { Id = "prod-03", Name = "Clavos 3\"", Code = "CLV-03", Unit = "pza", Price = 0.12m }
        //    };
        //}

        //// Unit DTOs (typed)
        //public class UnitResponse
        //{
        //    public string Id { get; set; }
        //    public string Name { get; set; }
        //}

        //public class UnitRequest
        //{
        //    public string Name { get; set; }
        //}
    }
}