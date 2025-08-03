using IntelliSoftAPIV2.Dtos.Proveedores;
using IntelliSoftAPIV2.Services.Proveedores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Proveedores
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedoresController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var proveedores = await _proveedorService.ObtenerTodosAsync();
            return Ok(proveedores);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var proveedor = await _proveedorService.ObtenerPorIdAsync(id);
            if (proveedor == null)
                return NotFound(new { message = "Proveedor no encontrado" });

            return Ok(proveedor);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProveedorCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _proveedorService.CrearAsync(dto);

            if (!resultado.Success)
            {
                return Conflict(new { message = resultado.Message });
            }

            return Ok(new { message = "Proveedor creado correctamente" });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProveedorUpdateDto dto)
        {
            try
            {
                var resultado = await _proveedorService.ActualizarAsync(id, dto);
                if (!resultado.Success)
                {
                    return NotFound(new { message = resultado.Message });
                }
                return Ok(new { message = "Proveedor actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {   
            try
            {
                var resultado = await _proveedorService.EliminarAsync(id);
                if (!resultado.Success)
                {
                    return NotFound(new { message = resultado.Message });
                }
                return Ok(new { message = "Proveedor eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
