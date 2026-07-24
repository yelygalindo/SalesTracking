using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Products.Comands;
using SalesTracking.Application.UseCases.Products.Interfaces;
using SalesTracking.Application.UseCases.Products.Models;
using SalesTracking.Application.UseCases.Products.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.Products;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Products;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ProductResponse>>> Get(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            ProductPagedList result = await _productService.GetAsync(
                new GetProductsRequest
                {
                    Search = search,
                    Page = page,
                    PageSize = pageSize
                }.ToApplication());

            return Ok(result.ToResponse());
        }

        [HttpGet("{externalId}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> GetByExternalId(string externalId)
        {
            ProductResult? result = await _productService.GetByExternalIdAsync(
                new GetProductByExternalIdCommand(externalId));

            if (result == null)
                return NotFound(new ErrorResponse { Error = "Producto no encontrado." });

            return Ok(result.ToResponse());
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdMessageResponse>> Create([FromBody] CreateProductRequest request)
        {
            CreateProductResult result = await _productService.CreateAsync(request.ToApplication());

            if (!result.Succeeded)
                return BadRequest(new ErrorResponse { Error = result.Message });

            return CreatedAtAction(
                nameof(GetByExternalId),
                new { externalId = result.Id },
                result.ToResponse());
        }

        [HttpPut("{externalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Update(
            string externalId,
            [FromBody] UpdateProductRequest request)
        {
            UpdateProductResult result = await _productService.UpdateAsync(
                request.ToApplication(externalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }

        [HttpDelete("{externalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Delete(string externalId)
        {
            DeleteProductResult result = await _productService.DeleteAsync(
                new DeleteProductCommand(externalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }
    }
}
