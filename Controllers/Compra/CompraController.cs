using IntelliSoftAPIV2.Dtos.Compra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IntelliSoftAPIV2.Services.Compra;

namespace IntelliSoftAPIV2.Controllers.Compra
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly CompraService _compraService;

        public CompraController(CompraService compraService)
        {
            _compraService = compraService;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CrearCompra([FromBody] CompraCreateDto dto)
        {
            var result = await _compraService.CrearCompra(dto);
            return Ok(new { success = result.Success, message = result.Message });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("Cancel/{id}")]
        public async Task<IActionResult> CancelarCompra(int id)
        {
            var result = await _compraService.CancelarCompra(id);
            return result.Success
                ? Ok(new { success = true, message = result.Message })
                : NotFound(new { success = false, message = result.Message });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> ListarCompras()
        {
            var result = await _compraService.ListarCompras();
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> ObtenerCompraPorId(int id)
        {
            var result = await _compraService.ObtenerCompraPorId(id);
            return result != null
                ? Ok(result)
                : NotFound(new { success = false, message = "Compra no encontrada" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("DetalleInventario/{id}")]
        public async Task<IActionResult> ObtenerDetallesParaInventario(int id)
        {
            var result = await _compraService.ObtenerDetalleInventarioPorId(id);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("NoInventariadas")]
        public async Task<IActionResult> ListarNoInventariadas()
        {
            var result = await _compraService.ListarComprasNoInventariadas();
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("Inventariar")]
        public async Task<IActionResult> InventariarCompra([FromBody] InventarioCreateDto dto)
        {
            var result = await _compraService.InventariarCompra(dto);
            return result.Success
                ? Ok(new { success = true, message = result.Message })
                : BadRequest(new { success = false, message = result.Message });
        }
    }
}
