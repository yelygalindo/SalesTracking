using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTracking.Application.UseCases.Deliveries.Interfaces;
using SalesTracking.Application.UseCases.Deliveries.Results;
using UrbanTrack.Api.Controllers.Requests.Mappers;
using UrbanTrack.Api.Controllers.Responses.Deliveries;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/delivery-statuses")]
    public sealed class DeliveryStatusesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryStatusesController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<DeliveryStatusResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DeliveryStatusResponse>>> Get()
        {
            IReadOnlyList<DeliveryStatusResult> result = await _deliveryService.GetStatusesAsync();
            return Ok(result.Select(x => x.ToResponse()).ToList());
        }
    }
}
