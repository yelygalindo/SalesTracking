using Microsoft.AspNetCore.Mvc;
using UrbanTrack.Api.Controllers.Responses.Dashboard;
using UrbanTrack.Api.Controllers.Responses.Common;
using Microsoft.AspNetCore.Http;

namespace UrbanTrack.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(DashboardMetricsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DashboardMetricsResponse>> Get()
        {
            var response = new DashboardMetricsResponse
            {
                Prospects = 24,
                ActiveCustomers = 132,
                ActiveProjects = 58,
                PendingDeliveries = 12,
                MonthlySales = 185430.50m,
                TodayFollowUps = 8,
                ActiveSellers = 6,
                RecentActivity = new List<string>
                {
                    "Entrega programada para Proyecto 'Centro Comercial Las Palmas'",
                    "Cliente 'Construcciones Rivera' convertido a activo",
                    "Nuevo prospecto: 'Aceros Norte'"
                },
                UpcomingFollowUps = new List<FollowUpItem>
                {
                    new FollowUpItem { CustomerId = "c-101", CustomerName = "Construcciones Rivera", When = DateTime.UtcNow.AddHours(3), Note = "Confirmar planos" },
                    new FollowUpItem { CustomerId = "c-205", CustomerName = "Almacen ObraS.A.", When = DateTime.UtcNow.AddDays(1), Note = "Enviar cotización" }
                }
            };

            return await Task.FromResult(Ok(response));
        }
    }
}