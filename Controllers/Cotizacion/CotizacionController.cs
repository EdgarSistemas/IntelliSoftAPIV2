using IntelliSoftAPIV2.Dtos.Cotizacion;
using IntelliSoftAPIV2.Services.Cotizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Cotizacion
{
    [Route("api/cotizacion")]
    [ApiController]
    public class CotizacionController : ControllerBase
    {
        private readonly CotizacionService _svc;

        public CotizacionController(CotizacionService svc)
        {
            _svc = svc;
        }

        // GET: api/cotizacion/resumen  (por PARTIDA)
        [HttpGet("resumen")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetResumen()
        {
            var result = await _svc.GetCotizacionesResumen();
            return Ok(result);
        }

        // POST: api/cotizacion/enviar-pdf
        [HttpPost("enviar-pdf")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EnviarPdf([FromBody] EnviarPdfCotizacionDto dto)
        {
            var result = await _svc.EnviarPdfCotizacion(dto);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message });
        }

        [HttpGet("pdf/{id:int}")]
        [Authorize] // ajusta si quieres AllowAnonymous
        public async Task<IActionResult> DescargarPdf(int id, [FromQuery] bool download = true)
        {
            var pdfBytes = await _svc.GenerarPdfBytes(id);
            if (pdfBytes == null)
                return NotFound("Cotización no encontrada o sin datos.");

            var dto = await _svc.GetCotizacionById(id);
            var fileName = $"Cotizacion-{dto?.ClaveCotizacion ?? id.ToString()}.pdf";

            Response.Headers["Content-Disposition"] =
                download
                    ? $"attachment; filename=\"{fileName}\""
                    : $"inline; filename=\"{fileName}\"";

            return File(pdfBytes, "application/pdf");
        }

        // GET: api/cotizacion/{id} (header + partidas + detalles)
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetCotizacion(int id)
        {
            var cot = await _svc.GetCotizacionById(id);
            if (cot == null) return NotFound("Cotización no encontrada.");
            return Ok(cot);
        }

        // POST: api/cotizacion/crear
        [HttpPost("crear")]
        [AllowAnonymous]
        public async Task<IActionResult> Crear([FromBody] CotizacionCreateDto dto)
        {
            var result = await _svc.CrearCotizacion(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, clave = result.Data });
        }

        // PUT: api/cotizacion/estado
        [HttpPut("estado")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CambiarEstado([FromBody] CotizacionEstadoUpdateDto dto)
        {
            var result = await _svc.ActualizarEstadoCotizacion(dto);
            if (!result.Success) return NotFound(new { message = result.Message });

            return Ok(new { success = true, message = result.Message });
        }

        // POST: api/cotizacion/aceptar
        [HttpPost("aceptar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Aceptar([FromBody] AceptarCotizacionDto dto)
        {
            var result = await _svc.AceptarCotizacion(dto);
            if (!result.Success) return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message });
        }
    }
}
