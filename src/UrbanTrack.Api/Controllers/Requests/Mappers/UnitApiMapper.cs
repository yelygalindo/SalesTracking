using SalesTracking.Application.UseCases.Units.Comands;
using SalesTracking.Application.UseCases.Units.Models;
using SalesTracking.Application.UseCases.Units.Results;
using UrbanTrack.Api.Controllers.Requests.Units;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Units;

namespace UrbanTrack.Api.Controllers.Requests.Mappers
{
    public static class UnitApiMapper
    {
        public static GetUnitsCommand ToApplication(this GetUnitsRequest request)
        {
            return new GetUnitsCommand(request.Search, request.Page, request.PageSize);
        }

        public static CreateUnitCommand ToApplication(this CreateUnitRequest request)
        {
            return new CreateUnitCommand
            {
                Name = request.Name,
                Symbol = request.Symbol,
                Description = request.Description,
                AllowsDecimals = request.AllowsDecimals,
                IsActive = request.IsActive
            };
        }

        public static UpdateUnitCommand ToApplication(
            this UpdateUnitRequest request,
            string externalId)
        {
            return new UpdateUnitCommand
            {
                ExternalId = externalId,
                Name = request.Name,
                Symbol = request.Symbol,
                Description = request.Description,
                AllowsDecimals = request.AllowsDecimals,
                IsActive = request.IsActive
            };
        }

        public static UnitResponse ToResponse(this UnitResult result)
        {
            return new UnitResponse
            {
                Id = result.Id,
                ExternalId = result.ExternalId,
                Name = result.Name,
                Symbol = result.Symbol,
                Description = result.Description,
                AllowsDecimals = result.AllowsDecimals,
                IsActive = result.IsActive,
                CreatedAtUtc = result.CreatedAtUtc,
                UpdatedAtUtc = result.UpdatedAtUtc
            };
        }

        public static PagedResponse<UnitResponse> ToResponse(this UnitPagedList result)
        {
            return new PagedResponse<UnitResponse>
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

        public static IdMessageResponse ToResponse(this CreateUnitResult result)
        {
            return new IdMessageResponse
            {
                Id = result.Id,
                Message = result.Message
            };
        }
    }
}