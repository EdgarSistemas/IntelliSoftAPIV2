using IntelliSoftAPIV2.Dtos.Unidades;
using IntelliSoftAPIV2.Services.Unidad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Unidad
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnidadController : ControllerBase
    {
        private readonly UnidadService _unidadService;

        public UnidadController(UnidadService unidadService)
        {
            _unidadService = unidadService;
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var unidades = await _unidadService.GetAll();
            return Ok(unidades);
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var unidad = await _unidadService.GetById(id);
            if (unidad == null) return NotFound("Unidad no encontrada");
            return Ok(unidad);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateUnidad([FromBody] UnidadCreateDto dto)
        {
            var result = await _unidadService.CreateUnidadAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(result.Data);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUnidad(int id, [FromBody] UnidadCreateDto dto)
        {
            var result = await _unidadService.UpdateUnidadAsync(id, dto);
            if (!result.Success) return NotFound(new { message = result.Message });
            return Ok(result.Data);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUnidad(int id)
        {
            var deleted = await _unidadService.DeleteUnidadAsync(id);
            if (!deleted) return NotFound("Unidad no encontrada");
            return Ok(new { message = "Unidad eliminada correctamente" });
        }
    }
}
