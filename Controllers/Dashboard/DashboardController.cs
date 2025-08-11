using System.Globalization;
using IntelliSoftAPIV2.Dtos.Dashboard;
using IntelliSoftAPIV2.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Dashboard
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _svc;

        public DashboardController(IDashboardService dashboardService)
        {
            _svc = dashboardService;
        }

        // ---- KPIs/Gráficas principales ----

        [Authorize(Roles = "admin")]
        [HttpGet("pedidosEstatus")]
        public async Task<ActionResult<List<PedidosEstatusDto>>> PedidosEstatus(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var data = await _svc.ObtenerPedidosEstatusAsync(from, to);
            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("ingresos-mensuales")]
        public async Task<ActionResult<List<MesValorDto>>> IngresosMensuales(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var data = await _svc.ObtenerIngresosMensualesAsync(from, to);
            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("pedidos-mensuales")]
        public async Task<ActionResult<List<MesValorDto>>> PedidosMensuales(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var data = await _svc.ObtenerPedidosMensualesAsync(from, to);
            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("top-productos")]
        public async Task<ActionResult<List<ProductoMasVendidoDto>>> TopProductos(
            [FromQuery] string metric = "ingreso", // "ingreso" | "partidas"
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int take = 10)
        {
            var data = await _svc.ObtenerTopProductosAsync(metric, from, to, take);
            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("clientes-top")]
        public async Task<ActionResult<List<ClienteTopDto>>> ClientesTop(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int take = 10)
        {
            var data = await _svc.ObtenerClientesTopAsync(from, to, take);
            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("conversion")]
        public async Task<ActionResult<ConversionDto>> Conversion(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var data = await _svc.ObtenerConversionAsync(from, to);
            return Ok(data);
        }

        // ---- Opiniones ----

        [Authorize(Roles = "admin")]
        [HttpGet("mejorOpinion")]
        public async Task<ActionResult<List<ProductoOpinionDto>>> ProductosMejorCalificados(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int take = 10)
        {
            var data = await _svc.ObtenerProductosMejorCalificadosAsync(from, to, take);
            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("distribucion")]
        public async Task<ActionResult<List<DistribucionOpinionDto>>> DistribucionOpiniones(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var data = await _svc.ObtenerDistribucionOpinionesAsync(from, to);
            return Ok(data);
        }
    }
}
