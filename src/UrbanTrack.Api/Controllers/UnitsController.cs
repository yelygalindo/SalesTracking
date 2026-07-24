using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Units.Comands;
using SalesTracking.Application.UseCases.Units.Interfaces;
using SalesTracking.Application.UseCases.Units.Models;
using SalesTracking.Application.UseCases.Units.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Requests.Units;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Pagination;
using UrbanTrack.Api.Controllers.Responses.Units;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/units")]
    public sealed class UnitsController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitsController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<UnitResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<UnitResponse>>> Get(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            UnitPagedList result = await _unitService.GetAsync(
                new GetUnitsRequest
                {
                    Search = search,
                    Page = page,
                    PageSize = pageSize
                }.ToApplication());

            return Ok(result.ToResponse());
        }

        [HttpGet("{unitExternalId}")]
        [ProducesResponseType(typeof(UnitResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UnitResponse>> GetByExternalId(string unitExternalId)
        {
            UnitResult? result = await _unitService.GetByExternalIdAsync(
                new GetUnitByExternalIdCommand(unitExternalId));

            if (result == null)
                return NotFound(new ErrorResponse { Error = "Unidad no encontrada." });

            return Ok(result.ToResponse());
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdMessageResponse>> Create([FromBody] CreateUnitRequest request)
        {
            CreateUnitResult result = await _unitService.CreateAsync(request.ToApplication());

            if (!result.Succeeded)
                return BadRequest(new ErrorResponse { Error = result.Message });

            return CreatedAtAction(
                nameof(GetByExternalId),
                new { unitExternalId = result.Id },
                result.ToResponse());
        }

        [HttpPut("{unitExternalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Update(
            string unitExternalId,
            [FromBody] UpdateUnitRequest request)
        {
            UpdateUnitResult result = await _unitService.UpdateAsync(
                request.ToApplication(unitExternalId));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }

        [HttpDelete("{unitExternalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Delete(string unitExternalId)
        {
            DeleteUnitResult result = await _unitService.DeleteAsync(
                new DeleteUnitCommand(unitExternalId));

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
