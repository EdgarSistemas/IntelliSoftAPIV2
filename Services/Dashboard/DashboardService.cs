using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        // Estatus: 0=Cancelado, 1=Pendiente, 2=EnProceso, 4=Pagado, 3=Finalizado
        private static readonly Dictionary<int, string> _estatusNombre = new()
        {
            { 1, "Pendiente" },
            { 2, "En proceso" },
            { 4, "Pagado" },
            { 3, "Finalizado" }
        };

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        private static (DateTime from, DateTime to) NormalizaRango(DateTime? from, DateTime? to)
        {
            var end = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1); // fin del día
            var start = (from ?? DateTime.UtcNow.AddDays(-90)).Date;        // últimos 90 días por default
            return (start, end);
        }

        private static decimal PrecioBasePartida(TbCotizacionProducto cp)
        {
            // ∑ (cantidad * precioPromedio) de los detalles "congelados"
            var basePrice = cp.Detalles.Sum(d => d.Cantidad * d.PrecioPromedio);
            return decimal.Round(basePrice, 2);
        }

        private static decimal PrecioVentaPartida(TbCotizacionProducto cp)
        {
            // Precio de venta al cliente (sin riesgo): base * (1 + %ganancia)
            var basePrice = PrecioBasePartida(cp);
            var conGan = basePrice * (1 + (cp.PorcentajeGanancia / 100m));
            return decimal.Round(conGan, 2);
        }

        // ----------------- Pedidos por estatus -----------------

        public async Task<List<PedidosEstatusDto>> ObtenerPedidosEstatusAsync(DateTime? from, DateTime? to)
        {
            var (start, end) = NormalizaRango(from, to);

            var datos = await _context.TbPedidos
                .Where(p => p.FechaPedido >= start && p.FechaPedido <= end && p.Estatus != 0)
                .GroupBy(p => p.Estatus)
                .Select(g => new { Estatus = g.Key, Total = g.Count() })
                .ToListAsync();

            // Mantenemos orden lógico
            var orden = new[] { 1, 2, 4, 3 };
            var result = new List<PedidosEstatusDto>();
            foreach (var e in orden)
            {
                result.Add(new PedidosEstatusDto
                {
                    EstatusNombre = _estatusNombre.TryGetValue(e, out var name) ? name : $"Estatus {e}",
                    Total = datos.FirstOrDefault(x => x.Estatus == e)?.Total ?? 0
                });
            }
            return result;
        }

        // ----------------- Ingresos por mes -----------------

        public async Task<List<MesValorDto>> ObtenerIngresosMensualesAsync(DateTime? from, DateTime? to)
        {
            var (start, end) = NormalizaRango(from, to);

            var crudos = await _context.TbPedidos
                .Where(p => p.FechaPedido >= start && p.FechaPedido <= end
                         && new[] { 2, 4, 3 }.Contains(p.Estatus)   // En proceso, Pagado, Finalizado
                         && p.Cotizacion != null)
                .SelectMany(p => p.Cotizacion.CotizacionProductos.Select(cp => new {
                    p.FechaPedido,
                    PrecioBase = cp.Detalles.Sum(d => d.Cantidad * d.PrecioPromedio),
                    cp.PorcentajeGanancia
                }))
                .Select(x => new {
                    Year = x.FechaPedido!.Value.Year,
                    Month = x.FechaPedido!.Value.Month,
                    PrecioVenta = Math.Round(x.PrecioBase * (1 + x.PorcentajeGanancia / 100m), 2)
                })
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new {
                    g.Key.Year,
                    g.Key.Month,
                    Total = g.Sum(x => x.PrecioVenta)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var resultado = crudos.Select(x => new MesValorDto
            {
                Periodo = $"{x.Year:D4}-{x.Month:D2}",
                Valor = x.Total
            }).ToList();

            return resultado;
        }

        // ----------------- #Pedidos por mes -----------------

        public async Task<List<MesValorDto>> ObtenerPedidosMensualesAsync(DateTime? from, DateTime? to)
        {
            var (start, end) = NormalizaRango(from, to);

            // 1) Todo lo traducible a SQL
            var crudos = await _context.TbPedidos
                .Where(p => p.FechaPedido >= start && p.FechaPedido <= end && p.Estatus != 0 && p.FechaPedido != null)
                .GroupBy(p => new { p.FechaPedido!.Value.Year, p.FechaPedido!.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Conteo = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var datos = crudos.Select(x => new MesValorDto
            {
                Periodo = $"{x.Year:D4}-{x.Month:D2}",
                Valor = x.Conteo
            }).ToList();

            return datos;
        }

        // ----------------- Top productos -----------------

        public async Task<List<ProductoMasVendidoDto>> ObtenerTopProductosAsync(
            string metric, DateTime? from, DateTime? to, int take)
        {
            metric = (metric ?? "ingreso").ToLowerInvariant();
            if (take <= 0) take = 10;

            var (start, end) = NormalizaRango(from, to);

            var pedidos = await _context.TbPedidos
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Detalles)
                .Where(p => p.FechaPedido >= start && p.FechaPedido <= end && p.Estatus >= 2)
                .ToListAsync();

            var query = pedidos
                .SelectMany(p => p.Cotizacion.CotizacionProductos)
                .Where(cp => cp.Producto != null)
                .GroupBy(cp => cp.Producto!.Nombre)
                .Select(g => new
                {
                    NombreProducto = g.Key,
                    Ingreso = g.Sum(PrecioVentaPartida),
                    Partidas = g.Count()
                });

            List<ProductoMasVendidoDto> result;
            if (metric == "partidas")
            {
                result = query
                    .OrderByDescending(x => x.Partidas)
                    .Take(take)
                    .Select(x => new ProductoMasVendidoDto
                    {
                        NombreProducto = x.NombreProducto,
                        TotalVendido = x.Partidas
                    })
                    .ToList();
            }
            else // ingreso
            {
                result = query
                    .OrderByDescending(x => x.Ingreso)
                    .Take(take)
                    .Select(x => new ProductoMasVendidoDto
                    {
                        NombreProducto = x.NombreProducto,
                        TotalVendido = x.Ingreso
                    })
                    .ToList();
            }

            return result;
        }

        // ----------------- Clientes top -----------------

        public async Task<List<ClienteTopDto>> ObtenerClientesTopAsync(DateTime? from, DateTime? to, int take)
        {
            if (take <= 0) take = 10;
            var (start, end) = NormalizaRango(from, to);

            var pedidos = await _context.TbPedidos
                .Include(p => p.Cotizacion).ThenInclude(c => c.Usuario)
                .Include(p => p.Cotizacion).ThenInclude(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Detalles)
                .Where(p => p.FechaPedido >= start && p.FechaPedido <= end && p.Estatus >= 2)
                .ToListAsync();

            var data = pedidos
                .GroupBy(p => new
                {
                    Id = p.Cotizacion.UsuarioId,
                    Nombre = p.Cotizacion.Usuario != null
                        ? (p.Cotizacion.Usuario.Nombre + " " + p.Cotizacion.Usuario.Apellidos)
                        : "(Sin nombre)"
                })
                .Select(g => new ClienteTopDto
                {
                    ClienteId = g.Key.Id ?? "",
                    NombreCliente = g.Key.Nombre ?? "",
                    Ingreso = g.Sum(p => p.Cotizacion.CotizacionProductos.Sum(PrecioVentaPartida))
                })
                .OrderByDescending(x => x.Ingreso)
                .Take(take)
                .ToList();

            return data;
        }

        // ----------------- Opiniones -----------------

        public async Task<List<ProductoOpinionDto>> ObtenerProductosMejorCalificadosAsync(
            DateTime? from, DateTime? to, int take)
        {
            if (take <= 0) take = 10;
            var (start, end) = NormalizaRango(from, to);

            var datos = await _context.TbOpiniones
                .Include(o => o.Producto)
                .Where(o => o.Estatus == 1
                            && o.ProductoId != null
                            && o.Calificacion != null
                            && o.Fecha >= start && o.Fecha <= end)
                .GroupBy(o => o.Producto)
                .Select(g => new ProductoOpinionDto
                {
                    NombreProducto = g.Key!.Nombre ?? "Sin nombre",
                    PromedioCalificacion = Math.Round(g.Average(x => x.Calificacion!.Value), 2),
                    TotalOpiniones = g.Count()
                })
                .OrderByDescending(x => x.PromedioCalificacion)
                .ThenByDescending(x => x.TotalOpiniones)
                .Take(take)
                .ToListAsync();

            return datos;
        }

        public async Task<List<DistribucionOpinionDto>> ObtenerDistribucionOpinionesAsync(DateTime? from, DateTime? to)
        {
            var (start, end) = NormalizaRango(from, to);

            var datos = await _context.TbOpiniones
                .Where(o => o.Estatus == 1
                            && o.Calificacion != null
                            && o.Fecha >= start && o.Fecha <= end)
                .GroupBy(o => o.Calificacion!.Value)
                .Select(g => new DistribucionOpinionDto
                {
                    Calificacion = g.Key,
                    Total = g.Count()
                })
                .ToListAsync();

            // asegurar 1..5 presentes
            var res = Enumerable.Range(1, 5)
                .Select(i => new DistribucionOpinionDto
                {
                    Calificacion = i,
                    Total = datos.FirstOrDefault(x => x.Calificacion == i)?.Total ?? 0
                })
                .ToList();

            return res;
        }

        // ----------------- Conversión -----------------

        public async Task<ConversionDto> ObtenerConversionAsync(DateTime? from, DateTime? to)
        {
            var (start, end) = NormalizaRango(from, to);

            var cotizaciones = await _context.TbCotizaciones
                .Where(c => c.FechaSolicitud >= start && c.FechaSolicitud <= end)
                .CountAsync();

            var pedidos = await _context.TbPedidos
                .Where(p => p.FechaPedido >= start && p.FechaPedido <= end)
                .CountAsync();

            return new ConversionDto
            {
                Cotizaciones = cotizaciones,
                Pedidos = pedidos,
                Conversion = cotizaciones > 0 ? Math.Round((decimal)pedidos / cotizaciones, 4) : 0m
            };
        }
    }
}