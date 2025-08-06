using System.Security.Claims;
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
        public async Task<IActionResult> CrearOpinion([FromBody] OpinionCreateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            await _service.CrearAsync(dto, userId);
            return Ok(new { message = "Opinión creada correctamente" });
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _service.ObtenerTodosAsync();
            return Ok(result);
        }

        [Authorize(Roles = "cliente")]
        [HttpGet("getByUser")]
        public async Task<IActionResult> ObtenerPorUsuario()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _service.ObtenerPorUsuarioAsync(userId);
            return Ok(result);
        }

        [HttpGet("getByProducto/{id}")]
        public async Task<IActionResult> ObtenerPorProducto(int id)
        {
            var result = await _service.ObtenerPorProductoAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "cliente")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> ActualizarOpinion(int id, [FromBody] OpinionCreateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var mensaje = await _service.ActualizarAsync(id, dto, userId);
            if (mensaje.Contains("permiso"))
                return Forbid(mensaje);

            return Ok(new { message = mensaje });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> EliminarOpinion(int id)
        {
            var mensaje = await _service.EliminarFisicaAsync(id);
            return Ok(new { message = mensaje });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("comentar")]
        public async Task<IActionResult> ComentarOpinion([FromBody] ComentarioCreateDto dto)
        {
            var mensaje = await _service.CrearComentarioAsync(dto);
            if (mensaje.Contains("ya tiene"))
                return BadRequest(new { message = mensaje });

            return Ok(new { message = mensaje });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("comentario/{id}")]
        public async Task<IActionResult> EliminarComentario(int id)
        {
            var mensaje = await _service.EliminarComentarioAsync(id);
            return Ok(new { message = mensaje });
        }
    }
}
