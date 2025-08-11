using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Cotizacion;
using IntelliSoftAPIV2.Dtos.Pedidos;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Pedidos
{
    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PedidoService> _logger;

        // Estados: 0=Cancelado, 1=Pendiente, 2=EnProceso, 4=Pagado, 3=Finalizado
        public const int ESTADO_CANCELADO = 0;
        public const int ESTADO_PENDIENTE = 1;
        public const int ESTADO_PROCESO = 2;
        public const int ESTADO_PAGADO = 4;
        public const int ESTADO_FINALIZADO = 3;

        public PedidoService(AppDbContext context, ILogger<PedidoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ----------------- Lecturas -----------------

        public async Task<List<PedidoResponseDto>> ObtenerTodosAsync()
        {
            var pedidos = await _context.TbPedidos
                .Include(p => p.Cotizacion).ThenInclude(c => c.Usuario)
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Detalles).ThenInclude(d => d.Insumo)
                .Where(p => p.Estatus != ESTADO_CANCELADO)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            var list = new List<PedidoResponseDto>();

            foreach (var pedido in pedidos)
            {
                var resp = new PedidoResponseDto
                {
                    IdPedido = pedido.IdPedido,
                    CotizacionId = pedido.CotizacionId,
                    CotizacionClave = pedido.Cotizacion.ClaveCotizacion,
                    FechaPedido = pedido.FechaPedido,
                    Estatus = pedido.Estatus,
                    ClienteId = pedido.Cotizacion.UsuarioId,
                    NombreCliente = pedido.Cotizacion.Usuario != null
                        ? $"{pedido.Cotizacion.Usuario.Nombre} {pedido.Cotizacion.Usuario.Apellidos}"
                        : "",
                    Comentario = pedido.Cotizacion.DetalleCotizacion,
                    Partidas = new List<CotizacionPartidaDto>()
                };

                foreach (var cp in pedido.Cotizacion.CotizacionProductos)
                {
                    var detDtos = cp.Detalles.Select(d => new CotizacionProductoDetalleDto
                    {
                        InsumoId = d.InsumoId,
                        NombreInsumo = d.Insumo?.Nombre ?? "",
                        Cantidad = d.Cantidad,
                        PrecioPromedio = d.PrecioPromedio
                    }).ToList();

                    var basePrice = decimal.Round(detDtos.Sum(d => d.Subtotal), 2);
                    var ganancia = decimal.Round(basePrice * (cp.PorcentajeGanancia / 100m), 2);
                    var conGan = decimal.Round(basePrice * (1 + cp.PorcentajeGanancia / 100m), 2);
                    var conRiesgo = cp.AplicaRiesgo == 1
                        ? decimal.Round(conGan * (1 + cp.PorcentajeRiesgo / 100m), 2)
                        : conGan;

                    resp.Partidas.Add(new CotizacionPartidaDto
                    {
                        CotizacionProductoId = cp.IdCotizacionProducto,
                        ProductoId = cp.ProductoId,
                        NombreProducto = cp.Producto?.Nombre,
                        Hectareas = cp.Hectareas,
                        PorcentajeGanancia = cp.PorcentajeGanancia,
                        PorcentajeRiesgo = cp.PorcentajeRiesgo,
                        AplicaRiesgo = cp.AplicaRiesgo,
                        Detalles = null,

                        PrecioBase = basePrice,
                        Ganancia = ganancia,
                        PrecioConGanancia = conGan,
                        PrecioConRiesgo = conRiesgo,
                        Total = conGan
                    });
                }

                // Totales
                resp.TotalPrecioBase = resp.Partidas.Sum(p => p.PrecioBase);
                resp.TotalGanancia = resp.Partidas.Sum(p => p.Ganancia);
                resp.TotalPrecioConGanancia = resp.Partidas.Sum(p => p.PrecioConGanancia);
                resp.TotalPrecioConRiesgo = resp.Partidas.Sum(p => p.PrecioConRiesgo);
                resp.Total = resp.Partidas.Sum(p => p.Total);

                list.Add(resp);
            }

            return list;
        }

        public async Task<PedidoResponseDto?> ObtenerPorIdAsync(int id)
        {
            var pedido = await _context.TbPedidos
                .Include(p => p.Cotizacion).ThenInclude(c => c.Usuario)
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Detalles).ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(p => p.IdPedido == id && p.Estatus != ESTADO_CANCELADO);

            if (pedido == null) return null;

            var resp = new PedidoResponseDto
            {
                IdPedido = pedido.IdPedido,
                CotizacionId = pedido.CotizacionId,
                CotizacionClave = pedido.Cotizacion.ClaveCotizacion,
                FechaPedido = pedido.FechaPedido,
                Estatus = pedido.Estatus,
                ClienteId = pedido.Cotizacion.UsuarioId,
                NombreCliente = pedido.Cotizacion.Usuario != null
                    ? $"{pedido.Cotizacion.Usuario.Nombre} {pedido.Cotizacion.Usuario.Apellidos}"
                    : "",
                Comentario = pedido.Cotizacion.DetalleCotizacion
            };

            // Partidas como en Cotización
            foreach (var cp in pedido.Cotizacion.CotizacionProductos)
            {
                var detDtos = cp.Detalles.Select(d => new CotizacionProductoDetalleDto
                {
                    InsumoId = d.InsumoId,
                    NombreInsumo = d.Insumo?.Nombre ?? "",
                    Cantidad = d.Cantidad,
                    PrecioPromedio = d.PrecioPromedio
                }).ToList();

                var basePrice = detDtos.Sum(d => d.Subtotal);
                basePrice = decimal.Round(basePrice, 2);

                var ganancia = decimal.Round(basePrice * (cp.PorcentajeGanancia / 100m), 2);
                var conGan = decimal.Round(basePrice * (1 + cp.PorcentajeGanancia / 100m), 2);
                var conRiesgo = cp.AplicaRiesgo == 1
                    ? decimal.Round(conGan * (1 + cp.PorcentajeRiesgo / 100m), 2)
                    : conGan;

                resp.Partidas.Add(new CotizacionPartidaDto
                {
                    CotizacionProductoId = cp.IdCotizacionProducto,
                    ProductoId = cp.ProductoId,
                    NombreProducto = cp.Producto?.Nombre,
                    Hectareas = cp.Hectareas,
                    PorcentajeGanancia = cp.PorcentajeGanancia,
                    PorcentajeRiesgo = cp.PorcentajeRiesgo,
                    AplicaRiesgo = cp.AplicaRiesgo,
                    Detalles = detDtos,

                    PrecioBase = basePrice,
                    Ganancia = ganancia,
                    PrecioConGanancia = conGan,
                    PrecioConRiesgo = conRiesgo,
                    Total = conGan
                });
            }

            // Totales consolidados
            resp.TotalPrecioBase = resp.Partidas.Sum(p => p.PrecioBase);
            resp.TotalGanancia = resp.Partidas.Sum(p => p.Ganancia);
            resp.TotalPrecioConGanancia = resp.Partidas.Sum(p => p.PrecioConGanancia);
            resp.TotalPrecioConRiesgo = resp.Partidas.Sum(p => p.PrecioConRiesgo);
            resp.Total = resp.Partidas.Sum(p => p.Total);

            return resp;
        }

        public async Task<List<PedidoResponseDto>> ObtenerPorUsuarioAsync(string usuarioId)
        {
            var pedidos = await _context.TbPedidos
                .Include(p => p.Cotizacion).ThenInclude(c => c.Usuario)
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Detalles).ThenInclude(d => d.Insumo)
                .Where(p => p.Cotizacion.UsuarioId == usuarioId && p.Estatus != ESTADO_CANCELADO)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            var list = new List<PedidoResponseDto>();

            foreach (var pedido in pedidos)
            {
                var resp = new PedidoResponseDto
                {
                    IdPedido = pedido.IdPedido,
                    CotizacionId = pedido.CotizacionId,
                    CotizacionClave = pedido.Cotizacion.ClaveCotizacion,
                    FechaPedido = pedido.FechaPedido,
                    Estatus = pedido.Estatus,
                    ClienteId = pedido.Cotizacion.UsuarioId,
                    NombreCliente = pedido.Cotizacion.Usuario != null
                        ? $"{pedido.Cotizacion.Usuario.Nombre} {pedido.Cotizacion.Usuario.Apellidos}"
                        : "",
                    Comentario = pedido.Cotizacion.DetalleCotizacion,
                    Partidas = new List<CotizacionPartidaDto>()
                };

                foreach (var cp in pedido.Cotizacion.CotizacionProductos)
                {
                    var detDtos = cp.Detalles.Select(d => new CotizacionProductoDetalleDto
                    {
                        InsumoId = d.InsumoId,
                        NombreInsumo = d.Insumo?.Nombre ?? "",
                        Cantidad = d.Cantidad,
                        PrecioPromedio = d.PrecioPromedio
                    }).ToList();

                    var basePrice = decimal.Round(detDtos.Sum(d => d.Subtotal), 2);
                    var ganancia = decimal.Round(basePrice * (cp.PorcentajeGanancia / 100m), 2);
                    var conGan = decimal.Round(basePrice * (1 + cp.PorcentajeGanancia / 100m), 2);
                    var conRiesgo = cp.AplicaRiesgo == 1
                        ? decimal.Round(conGan * (1 + cp.PorcentajeRiesgo / 100m), 2)
                        : conGan;

                    resp.Partidas.Add(new CotizacionPartidaDto
                    {
                        CotizacionProductoId = cp.IdCotizacionProducto,
                        ProductoId = cp.ProductoId,
                        NombreProducto = cp.Producto?.Nombre,
                        Hectareas = cp.Hectareas,
                        PorcentajeGanancia = cp.PorcentajeGanancia,
                        PorcentajeRiesgo = cp.PorcentajeRiesgo,
                        AplicaRiesgo = cp.AplicaRiesgo,
                        Detalles = detDtos,

                        PrecioBase = basePrice,
                        Ganancia = ganancia,
                        PrecioConGanancia = conGan,
                        PrecioConRiesgo = conRiesgo,
                        Total = conGan
                    });
                }

                // Totales
                resp.TotalPrecioBase = resp.Partidas.Sum(p => p.PrecioBase);
                resp.TotalGanancia = resp.Partidas.Sum(p => p.Ganancia);
                resp.TotalPrecioConGanancia = resp.Partidas.Sum(p => p.PrecioConGanancia);
                resp.TotalPrecioConRiesgo = resp.Partidas.Sum(p => p.PrecioConRiesgo);
                resp.Total = resp.Partidas.Sum(p => p.Total);

                list.Add(resp);
            }

            return list;
        }

        // ----------------- Acciones -----------------

        public async Task<ServiceResult<string>> CancelarAsync(int id)
        {
            var pedido = await _context.TbPedidos.FirstOrDefaultAsync(p => p.IdPedido == id);
            if (pedido == null) return ServiceResult<string>.Failure("Pedido no encontrado.");

            if (pedido.Estatus != ESTADO_PENDIENTE)
                return ServiceResult<string>.Failure("Solo se pueden cancelar pedidos en estado 'Pendiente'.");

            pedido.Estatus = ESTADO_CANCELADO;
            await _context.SaveChangesAsync();
            return ServiceResult<string>.CreateSuccess("Pedido cancelado.");
        }

        public async Task<ServiceResult<string>> ProcesarAsync(int id)
        {
            var pedido = await _context.TbPedidos
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null) return ServiceResult<string>.Failure("Pedido no encontrado.");
            if (pedido.Estatus != ESTADO_PENDIENTE)
                return ServiceResult<string>.Failure("El pedido debe estar 'Pendiente'.");

            // Sumar requerimientos por insumo (todas las partidas)
            var requeridos = new Dictionary<int, decimal>(); // insumoId -> cantidad
            foreach (var cp in pedido.Cotizacion.CotizacionProductos)
            {
                foreach (var d in cp.Detalles)
                {
                    if (!requeridos.ContainsKey(d.InsumoId)) requeridos[d.InsumoId] = 0m;
                    requeridos[d.InsumoId] += d.Cantidad;
                }
            }

            // Validar existencias
            var faltantes = new List<string>();
            foreach (var kv in requeridos)
            {
                int insumoId = kv.Key; decimal req = kv.Value;

                var ultimo = await _context.TbInventarioInsumos
                    .Where(i => i.InsumoId == insumoId)
                    .OrderByDescending(i => i.Fecha)
                    .FirstOrDefaultAsync();

                var exist = (ultimo?.Existencias ?? 0);
                if (exist < req) faltantes.Add($"Insumo {insumoId} (req: {req}, disp: {exist})");
            }

            if (faltantes.Count > 0)
                return ServiceResult<string>.Failure("No hay existencias suficientes: " + string.Join("; ", faltantes));

            // Registrar salidas y pasar a EnProceso
            foreach (var kv in requeridos)
            {
                int insumoId = kv.Key; decimal salida = kv.Value;

                var ultimo = await _context.TbInventarioInsumos
                    .Where(i => i.InsumoId == insumoId)
                    .OrderByDescending(i => i.Fecha)
                    .FirstOrDefaultAsync();

                decimal costo = ultimo?.Costo ?? 0m;
                decimal haber = salida * costo;
                decimal saldoAnt = ultimo?.Saldo ?? 0m;
                decimal nuevoSaldo = saldoAnt - haber;
                decimal existAnt = ultimo?.Existencias ?? 0m;
                decimal nuevasExist = existAnt - salida;

                var mov = new TbInventarioInsumo
                {
                    InsumoId = insumoId,
                    Fecha = DateTime.Now,
                    Entrada = null,
                    Salida = (int?)salida,                     // tu modelo usa int? para Salida
                    Existencias = (int?)nuevasExist,
                    Costo = costo,
                    Promedio = null,
                    Debe = null,
                    Haber = haber,
                    Saldo = nuevoSaldo,
                    PedidoId = pedido.IdPedido,
                    CompraId = null
                };
                _context.TbInventarioInsumos.Add(mov);
            }

            pedido.Estatus = ESTADO_PROCESO;
            await _context.SaveChangesAsync();
            return ServiceResult<string>.CreateSuccess("Pedido procesado. Se registraron las salidas y el estado es 'En proceso'.");
        }

        public async Task<ServiceResult<string>> MarcarPagadoPorClienteAsync(int id)
        {
            var pedido = await _context.TbPedidos.FirstOrDefaultAsync(p => p.IdPedido == id);
            if (pedido == null) return ServiceResult<string>.Failure("Pedido no encontrado.");

            if (pedido.Estatus != ESTADO_PROCESO)
                return ServiceResult<string>.Failure("Solo se puede marcar pagado si el pedido está 'En proceso'.");

            pedido.Estatus = ESTADO_PAGADO;
            await _context.SaveChangesAsync();
            return ServiceResult<string>.CreateSuccess("Pedido marcado como 'Pagado'.");
        }

        public async Task<ServiceResult<string>> FinalizarAsync(int id)
        {
            var pedido = await _context.TbPedidos.FirstOrDefaultAsync(p => p.IdPedido == id);
            if (pedido == null) return ServiceResult<string>.Failure("Pedido no encontrado.");

            if (pedido.Estatus != ESTADO_PAGADO)
                return ServiceResult<string>.Failure("Solo se puede finalizar un pedido 'Pagado'.");

            pedido.Estatus = ESTADO_FINALIZADO;
            await _context.SaveChangesAsync();
            return ServiceResult<string>.CreateSuccess("Pedido finalizado.");
        }
    }
}
