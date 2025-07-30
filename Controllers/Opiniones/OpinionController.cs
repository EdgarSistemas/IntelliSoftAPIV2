using IntelliSoftAPIV2.Dtos.Opiniones;
using IntelliSoftAPIV2.Services.Opiniones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Opiniones
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpinionController : ControllerBase
    {
        private readonly IOpinionService _service;

        public OpinionController(IOpinionService service)
        {
            _service = service;
        }

        [Authorize(Roles = "cliente")]
        [HttpPost("create")]
        public async Task<IActionResult> Crear([FromBody] OpinionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.CrearAsync(dto);
            return Ok(new { message = "Opinión registrada correctamente" });
        }

        [Authorize(Roles = "cliente,admin")]
        [HttpGet("getAll")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var opiniones = await _service.ObtenerTodosAsync();
            return Ok(opiniones);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var mensaje = await _service.EliminarAsync(id);

            if (mensaje.Contains("no encontrada") || mensaje.Contains("ya estaba eliminada"))
                return NotFound(new { message = mensaje });

            return Ok(new { message = mensaje });
        }
    }
}
