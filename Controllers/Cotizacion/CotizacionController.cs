using IntelliSoftAPIV2.Dtos.Cotizacion;
using IntelliSoftAPIV2.Services.Cotizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Cotizacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class CotizacionController : ControllerBase
    {
        private readonly CotizacionService _cotizacionService;

        public CotizacionController(CotizacionService cotizacionService)
        {
            _cotizacionService = cotizacionService;
        }

        [HttpGet("resumen")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetResumen()
        {
            var result = await _cotizacionService.GetCotizacionesResumen();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCotizacion(int id)
        {
            var cot = await _cotizacionService.GetCotizacionById(id);
            if (cot == null) return NotFound();
            return Ok(cot);
        }

        [HttpPost("crear")]
        [AllowAnonymous]
        public async Task<IActionResult> Crear([FromBody] CotizacionCreateDto dto)
        {
            var success = await _cotizacionService.CrearCotizacion(dto);
            if (!success) return BadRequest("Error al crear cotización");
            return Ok(new { message = "Cotización registrada correctamente" });
        }

        [HttpPut("estado")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CambiarEstado([FromBody] CotizacionEstadoUpdateDto dto)
        {
            var success = await _cotizacionService.ActualizarEstadoCotizacion(dto);
            if (!success) return NotFound("Cotización no encontrada");
            return Ok(new { message = "Estado actualizado correctamente" });
        }

        [HttpPost("aceptar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Aceptar([FromBody] AceptarCotizacionDto dto)
        {
            var success = await _cotizacionService.AceptarCotizacion(dto);
            if (!success) return BadRequest("No se pudo aceptar la cotización");
            return Ok(new { message = "Cotización aceptada y pedido generado" });
        }
    }

}
