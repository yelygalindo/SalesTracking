using SalesTracking.Application.UseCases.Products.Comands;
using SalesTracking.Application.UseCases.Products.Models;
using SalesTracking.Application.UseCases.Products.Results;
using UrbanTrack.Api.Controllers.Requests.Products;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Products;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class ProductApiMapper
    {
        public static GetProductsCommand ToApplication(this GetProductsRequest request)
        {
            return new GetProductsCommand(
                request.Search,
                request.Page,
                request.PageSize);
        }

        public static CreateProductCommand ToApplication(this CreateProductRequest request)
        {
            return new CreateProductCommand
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                ExternalUnitId = request.ExternalUnitId,
                Price = request.Price,
                IsActive = request.IsActive
            };
        }

        public static UpdateProductCommand ToApplication(
            this UpdateProductRequest request,
            string externalId)
        {
            return new UpdateProductCommand
            {
                ExternalId = externalId,
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                ExternalUnitId = request.ExternalUnitId,
                Price = request.Price,
                IsActive = request.IsActive
            };
        }

        public static ProductResponse ToResponse(this ProductResult result)
        {
            return new ProductResponse
            {
                Id = result.Id,
                ExternalId = result.ExternalId,
                Code = result.Code,
                Name = result.Name,
                Description = result.Description,
                ExternalUnitId = result.ExternalUnitId,
                Price = result.Price,
                IsActive = result.IsActive,
                CreatedAtUtc = result.CreatedAtUtc,
                UpdatedAtUtc = result.UpdatedAtUtc
            };
        }

        public static PagedResponse<ProductResponse> ToResponse(this ProductPagedList result)
        {
            return new PagedResponse<ProductResponse>
            {
                Items = result.Items.Select(x => x.ToResponse()).ToList(),
                Pagination = new PaginationResponse
                {
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                }
            };
        }

        public static IdMessageResponse ToResponse(this CreateProductResult result)
        {
            return new IdMessageResponse
            {
                Id = result.Id,
                Message = result.Message
            };
        }
    }
}
