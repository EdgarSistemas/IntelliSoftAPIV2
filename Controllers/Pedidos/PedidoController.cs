using System.Security.Claims;
using IntelliSoftAPIV2.Dtos.Pedidos;
using IntelliSoftAPIV2.Services;
using IntelliSoftAPIV2.Services.Pedidos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Pedidos
{
    [Route("api/pedidos")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _service;

        public PedidoController(IPedidoService service)
        {
            _service = service;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pedidos = await _service.ObtenerTodosAsync();
            if (pedidos == null || !pedidos.Any())
                return NotFound(new { message = "No se encontraron pedidos." });
            return Ok(pedidos);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pedido = await _service.ObtenerPorIdAsync(id);
            return pedido == null
                ? NotFound(new { message = $"No se encontró el pedido {id}." })
                : Ok(pedido);
        }

        // ---- Flujo nuevo ----

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var r = await _service.CancelarAsync(id);
            return r.Success ? Ok(new { message = r.Data }) : BadRequest(new { message = r.Message });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}/procesar")]
        public async Task<IActionResult> Procesar(int id)
        {
            var r = await _service.ProcesarAsync(id);
            return r.Success ? Ok(new { message = r.Data }) : BadRequest(new { message = r.Message });
        }

        // Lo invoca el cliente autenticado
        [Authorize(Roles = "cliente")]
        [HttpPut("{id:int}/pagado")]
        public async Task<IActionResult> MarcarPagadoPorCliente(int id)
        {
            // (Puedes validar que el pedido pertenezca al usuario corriente)
            var r = await _service.MarcarPagadoPorClienteAsync(id);
            return r.Success ? Ok(new { message = r.Data }) : BadRequest(new { message = r.Message });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}/finalizar")]
        public async Task<IActionResult> Finalizar(int id)
        {
            var r = await _service.FinalizarAsync(id);
            return r.Success ? Ok(new { message = r.Data }) : BadRequest(new { message = r.Message });
        }

        // ---- Vista cliente ----
        [Authorize(Roles = "admin,cliente")]
        [HttpGet("cliente")]
        public async Task<IActionResult> ObtenerPedidosPorCliente()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized("No se pudo obtener el ID de usuario.");
            var pedidos = await _service.ObtenerPorUsuarioAsync(userId);
            return Ok(pedidos);
        }
    }
}
