using InventoryDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var dto = await _dashboardService.GetOverviewAsync();
            return Ok(dto);
        }
    }
}