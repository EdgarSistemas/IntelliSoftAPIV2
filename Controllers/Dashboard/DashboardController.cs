using IntelliSoftAPIV2.Dtos.Dashboard;
using IntelliSoftAPIV2.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("pedidosEstatus")]
        public async Task<ActionResult<List<PedidosEstatusDto>>> ObtenerPedidosPorEstatus()
        {
            var resultado = await _dashboardService.ObtenerPedidosEstatusAsync();
            return Ok(resultado);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("masVendidos")]
        public async Task<IActionResult> ObtenerProductosMasVendidos()
        {
            var productos = await _dashboardService.ObtenerProductosMasVendidosAsync();
            return Ok(new { data = productos });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("mejorOpinion")]
        public async Task<IActionResult> ObtenerProductosMejorCalificados()
        {
            var resultado = await _dashboardService.ObtenerProductosMejorCalificadosAsync();
            return Ok(resultado);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("distribucion")]
        public async Task<IActionResult> ObtenerDistribucionOpiniones()
        {
            var resultado = await _dashboardService.ObtenerDistribucionOpinionesAsync();
            return Ok(resultado);
        }
    }
}
