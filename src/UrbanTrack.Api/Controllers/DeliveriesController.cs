using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.Common.Interfaces;
using SalesTracking.Application.UseCases.Deliveries.Comands;
using SalesTracking.Application.UseCases.Deliveries.Interfaces;
using SalesTracking.Application.UseCases.Deliveries.Models;
using SalesTracking.Application.UseCases.Deliveries.Results;
using UrbanTrack.Api.Controllers.Requests.Deliveries;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.Common;
using UrbanTrack.Api.Controllers.Responses.Deliveries;
using UrbanTrack.Api.Controllers.Responses.Pagination;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/deliveries")]
    public sealed class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly ICurrentUser _currentUser;

        public DeliveriesController(IDeliveryService deliveryService, ICurrentUser currentUser)
        {
            _deliveryService = deliveryService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<DeliveryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<DeliveryResponse>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            DeliveryPagedList result = await _deliveryService.GetAsync(
                new GetDeliveriesRequest
                {
                    Page = page,
                    PageSize = pageSize
                }.ToApplication());

            return Ok(result.ToResponse());
        }

        [HttpGet("{deliveryExternalId}")]
        [ProducesResponseType(typeof(DeliveryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeliveryResponse>> GetByExternalId(string deliveryExternalId)
        {
            DeliveryResult? result = await _deliveryService.GetByExternalIdAsync(
                new GetDeliveryByExternalIdCommand(deliveryExternalId));

            if (result == null)
                return NotFound(new ErrorResponse { Error = "Entrega no encontrada." });

            return Ok(result.ToResponse());
        }

        [HttpPost]
        [ProducesResponseType(typeof(IdMessageResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IdMessageResponse>> Create([FromBody] CreateDeliveryRequest request)
        {
            CreateDeliveryResult result = await _deliveryService.CreateAsync(request.ToApplication(_currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Created(string.Empty, result.ToResponse());
        }

        [HttpPut("{deliveryExternalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Update(
            string deliveryExternalId,
            [FromBody] UpdateDeliveryRequest request)
        {
            UpdateDeliveryResult result = await _deliveryService.UpdateAsync(
                request.ToApplication(deliveryExternalId, _currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }

        [HttpPatch("{deliveryExternalId}/status")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> ChangeStatus(
            string deliveryExternalId,
            [FromBody] ChangeDeliveryStatusRequest request)
        {
            ChangeDeliveryStatusResult result = await _deliveryService.ChangeStatusAsync(
                request.ToApplication(deliveryExternalId, _currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }


        [HttpPost("{deliveryExternalId}/receipts")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> ConfirmReceipt(
            string deliveryExternalId,
            [FromBody] ConfirmDeliveryReceiptRequest request)
        {
            ConfirmDeliveryReceiptResult result = await _deliveryService.ConfirmReceiptAsync(
                request.ToApplication(deliveryExternalId, _currentUser.UserId.GetValueOrDefault()));

            if (!result.Succeeded)
            {
                if (result.NotFound)
                    return NotFound(new ErrorResponse { Error = result.Message });

                return BadRequest(new ErrorResponse { Error = result.Message });
            }

            return Ok(new MessageResponse { Message = result.Message });
        }
        [HttpDelete("{deliveryExternalId}")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MessageResponse>> Delete(string deliveryExternalId)
        {
            DeleteDeliveryResult result = await _deliveryService.DeleteAsync(
                new DeleteDeliveryCommand(deliveryExternalId, _currentUser.UserId.GetValueOrDefault()));

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
