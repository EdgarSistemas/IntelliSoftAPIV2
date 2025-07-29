using IntelliSoftAPIV2.Dtos.Insumos;
using IntelliSoftAPIV2.Services.Insumo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Insumo
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsumoController : ControllerBase
    {
        private readonly InsumoService _insumoService;

        public InsumoController(InsumoService insumoService)
        {
            _insumoService = insumoService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var insumos = await _insumoService.GetAll();
            return Ok(insumos);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var insumo = await _insumoService.GetById(id);
            if (insumo == null) return NotFound("Insumo no encontrado");
            return Ok(insumo);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] InsumoCreateDto dto)
        {
            var result = await _insumoService.CreateAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(result.Data);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] InsumoUpdateDto dto)
        {
            var result = await _insumoService.UpdateAsync(id, dto);
            if (!result.Success) return NotFound(new { message = result.Message });
            return Ok(result.Data);
        }

        [HttpDelete("Delete{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _insumoService.DeleteAsync(id);
            if (!deleted) return NotFound("Insumo no encontrado");
            return Ok(new { message = "Insumo eliminado correctamente" });
        }
    }
}
