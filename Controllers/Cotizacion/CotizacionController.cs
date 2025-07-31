using IntelliSoftAPIV2.Dtos.Cotizacion;
using IntelliSoftAPIV2.Services.Cotizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        // GET: api/cotizacion/resumen
        [HttpGet("resumen")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetResumen()
        {
            var result = await _cotizacionService.GetCotizacionesResumen();
            return Ok(result);
        }

        // GET: api/cotizacion/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCotizacion(int id)
        {
            var cot = await _cotizacionService.GetCotizacionById(id);
            if (cot == null)
                return NotFound("Cotización no encontrada");
            return Ok(cot);
        }

        [HttpPost("crear")]
        [AllowAnonymous]
        public async Task<IActionResult> Crear([FromBody] CotizacionCreateDto dto)
        {
            var result = await _cotizacionService.CrearCotizacion(dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                clave = result.Data
            });
        }

        [HttpPut("estado")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CambiarEstado([FromBody] CotizacionEstadoUpdateDto dto)
        {
            var result = await _cotizacionService.ActualizarEstadoCotizacion(dto);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return result.Success
                 ? Ok(new { success = true, message = result.Message })
                 : BadRequest(new { success = false, message = result.Message });
        }

        // POST: api/cotizacion/aceptar
        [HttpPost("aceptar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Aceptar([FromBody] AceptarCotizacionDto dto)
        {
            var success = await _cotizacionService.AceptarCotizacion(dto);
            if (!success.Success)
                return BadRequest("No se pudo aceptar la cotización");

            return success.Success
                 ? Ok(new { success = true, message = success.Message })
                 : BadRequest(new { success = false, message = success.Message });
        }
    }

}
