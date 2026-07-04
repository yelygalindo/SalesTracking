using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/customer-timeline")]
    public class CustomerTimelineController : ControllerBase    
    {
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
    }
}
