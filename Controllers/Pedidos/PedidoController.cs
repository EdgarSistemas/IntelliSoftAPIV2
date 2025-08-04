using System.Security.Claims;
using IntelliSoftAPIV2.Dtos.Pedidos;
using IntelliSoftAPIV2.Services;
using IntelliSoftAPIV2.Services.Pedidos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Pedidos
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _service;

        public PedidoController(IPedidoService service)
        {
            _service = service;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pedidos = await _service.ObtenerTodosAsync();

                if (pedidos == null || !pedidos.Any())
                {
                    return NotFound(new { message = "No se encontraron pedidos." });
                }

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error interno.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pedido = await _service.ObtenerPorIdAsync(id);

                if (pedido == null)
                {
                    return NotFound(new { message = $"No se encontró el pedido con Id {id}." });
                }

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                // Aquí puedes hacer logging del error si tienes un sistema
                return StatusCode(500, new { message = "Ocurrió un error interno.", detail = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var resultado = await _service.EliminarAsync(id);

            if (!resultado.Success)
                return NotFound(new { mensaje = resultado.Message });

            return Ok(new { message = "Pedido eliminado correctamente" });
        }


        [Authorize(Roles = "admin")]
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarEstatus(int id, [FromBody] PedidoUpdateDto dto)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "ID inválido." });

            ServiceResult<string> resultado;

            if (dto.Estatus == 2)
            {
                resultado = await _service.EstatusProcesoAsync(id, dto.Estatus);
            }
            else if (dto.Estatus == 3)
            {
                resultado = await _service.CompletarPedidoAsync(id);
            }
            else
            {
                return BadRequest(new { mensaje = "Estatus no soportado. Solo se aceptan 2 (en proceso) o 3 (completado)." });
            }

            if (!resultado.Success)
                return NotFound(new { mensaje = resultado.Message });

            return Ok(new { mensaje = resultado.Data });
        }

        [Authorize(Roles = "admin,cliente")]
        [HttpGet("cliente")]
        public async Task<IActionResult> ObtenerPedidosPorCliente()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine(userId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el ID del usuario autenticado");

            var pedidos = await _service.ObtenerPorUsuarioAsync(userId);

            return Ok(pedidos);
        }


    }
}
