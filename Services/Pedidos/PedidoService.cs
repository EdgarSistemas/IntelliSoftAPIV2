using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Cotizacion;
using IntelliSoftAPIV2.Dtos.Pedidos;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Pedidos
{
    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _context;

        public PedidoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PedidoResponseDto>> ObtenerTodosAsync()
        {
            return await _context.TbPedidos
            .Include(p => p.Cotizacion)
                .ThenInclude(c => c.Usuario)
            .Where(p => p.Estatus != 0)
            .Select(p => new PedidoResponseDto
            {
                IdPedido = p.IdPedido,
                CotizacionId = p.CotizacionId,
                FechaPedido = p.FechaPedido,
                ClienteId = p.Cotizacion.UsuarioId,
                NombreCliente = p.Cotizacion.Usuario.Nombre + " " + p.Cotizacion.Usuario.Apellidos,
                Comentario = p.Cotizacion.DetalleCotizacion,
                Estatus = p.Estatus,
            }).ToListAsync();

        }

        public async Task<PedidoResponseDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.TbPedidos
                .Include(p => p.Cotizacion)
                    .ThenInclude(c => c.Detalles)
                .Include(p => p.Cotizacion)
                    .ThenInclude(c => c.Usuario)
                .Where(p => p.IdPedido == id && p.Estatus != 0)
                .Select(p => new PedidoResponseDto
                {
                    IdPedido = p.IdPedido,
                    CotizacionId = p.CotizacionId,
                    FechaPedido = p.FechaPedido,
                    ClienteId = p.Cotizacion.UsuarioId,
                    NombreCliente = p.Cotizacion.Usuario.Nombre + " " + p.Cotizacion.Usuario.Apellidos,
                    Comentario = p.Cotizacion.DetalleCotizacion,
                    Estatus = p.Estatus,
                    Detalles = p.Cotizacion.Detalles.Select(d => new CotizacionDetalleDto
                    {
                        Cantidad = d.Cantidad,
                        PrecioPromedio = d.PrecioPromedio
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<string>> EliminarAsync(int id)
        {
            var pedido = await _context.TbPedidos.FirstOrDefaultAsync(p => p.IdPedido == id && p.Estatus != 0);

            if (pedido == null)
                return ServiceResult<string>.Failure("Pedido no encontrado o ya fue eliminado.");

            pedido.Estatus = 0;
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess("Pedido eliminado correctamente.");
        }

        public async Task<ServiceResult<string>> EstatusProcesoAsync(int id, int nuevoEstatus)
        {
            if (id <= 0)
                return ServiceResult<string>.Failure("ID de pedido inválido.");
            
            if (nuevoEstatus < 1 || nuevoEstatus > 3)
                return ServiceResult<string>.Failure("Estatus invalido.");

            var pedido = await _context.TbPedidos
                .Include(p => p.Cotizacion)
                    .ThenInclude(c => c.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == id && p.Estatus == 1);

            if (pedido == null)
                return ServiceResult<string>.Failure("Pedido no encontrado o eliminado.");

            // Validar existencias solo si el nuevo estatus es '2' (En proceso)
            if (nuevoEstatus == 2)
            {
                foreach (var detalle in pedido.Cotizacion.Detalles)
                {
                    var insumo = await _context.TbInventarioInsumos
                        .Where(i => i.InsumoId == detalle.InsumoId)
                        .OrderByDescending(i => i.Fecha)
                        .FirstOrDefaultAsync();

                    if (insumo == null)
                        return ServiceResult<string>.Failure($"El insumo con ID {detalle.InsumoId} no existe o está inactivo.");

                    if (insumo.Existencias < detalle.Cantidad)
                        return ServiceResult<string>.Failure($"No hay suficientes existencias del insumo (Requiere: {detalle.Cantidad}, Disponibles: {insumo.Existencias}).");
                }
            }

            // Si pasa todas las validaciones, actualiza el estatus
            pedido.Estatus = nuevoEstatus;

            _context.TbPedidos.Update(pedido);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess("Estatus actualizado correctamente.");
        }

        public async Task<ServiceResult<string>> CompletarPedidoAsync(int id)
        {
            var pedido = await _context.TbPedidos
                .Include(p => p.Cotizacion)
                    .ThenInclude(c => c.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == id && p.Estatus == 2);

            if (pedido == null)
                return ServiceResult<string>.Failure("Pedido no encontrado o no está en proceso.");

            foreach (var detalle in pedido.Cotizacion.Detalles)
            {
                var ultimoRegistro = await _context.TbInventarioInsumos
                    .Where(i => i.InsumoId == detalle.InsumoId)
                    .OrderByDescending(i => i.Fecha)
                    .FirstOrDefaultAsync();

                if (ultimoRegistro == null)
                    return ServiceResult<string>.Failure($"No existe historial del insumo {detalle.InsumoId}");

                if (ultimoRegistro.Existencias < detalle.Cantidad)
                    return ServiceResult<string>.Failure($"Insumo {detalle.InsumoId} insuficiente. Disponibles: {ultimoRegistro.Existencias}");

                // Cálculos
                decimal salida = detalle.Cantidad;
                decimal costo = ultimoRegistro.Costo ?? 0;
                decimal haber = salida * costo;
                decimal saldoAnterior = ultimoRegistro.Saldo ?? 0;
                decimal nuevoSaldo = saldoAnterior - haber;
                decimal nuevasExistencias = (ultimoRegistro.Existencias?? 0) - salida;

                // Crear nuevo registro de salida
                var nuevoMovimiento = new TbInventarioInsumo
                {
                    InsumoId = detalle.InsumoId,
                    Fecha = DateTime.Now,
                    Entrada = 0,
                    Salida = (int?)salida,
                    Existencias = (int?)nuevasExistencias,
                    Costo = costo,
                    Haber = haber,
                    Saldo = nuevoSaldo
                };

                _context.TbInventarioInsumos.Add(nuevoMovimiento);
            }

            pedido.Estatus = 3;
            _context.TbPedidos.Update(pedido);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess("Pedido completado y salidas registradas.");
        }


    }
}
