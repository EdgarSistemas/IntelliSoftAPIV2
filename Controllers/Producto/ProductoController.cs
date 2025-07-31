using IntelliSoftAPIV2.Dtos.Productos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IntelliSoftAPIV2.Services.Producto;

namespace IntelliSoftAPIV2.Controllers.Producto
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _productoService;

        public ProductoController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _productoService.GetAll();
            return Ok(productos);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _productoService.GetById(id);
            if (producto == null) return NotFound("Producto no encontrado");
            return Ok(producto);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
        {
            var result = await _productoService.Create(dto);
            return result.Success
                ? Ok(new { success = true, message = result.Message })
                : BadRequest(new { success = false, message = result.Message });
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoCreateDto dto)
        {
            var result = await _productoService.Update(id, dto);
            return result.Success
                ? Ok(new { success = true, message = result.Message })
                : BadRequest(new { success = false, message = result.Message });
        }


        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productoService.Delete(id);
            return result.Success
                ? Ok(new { success = true, message = result.Message })
                : NotFound(new { success = false, message = result.Message });
        }
    }
}
