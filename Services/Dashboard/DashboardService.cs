using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Dashboard
{
    //public class DashboardService : IDashboardService
    //{
    //    private readonly AppDbContext _context;

    //    public DashboardService(AppDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<List<PedidosEstatusDto>> ObtenerPedidosEstatusAsync()
    //    {
    //        var estatusDict = new Dictionary<int, string>
    //        {
    //            { 1, "Pendiente" },
    //            { 2, "En proceso" },
    //            { 3, "Completado" }
    //        };

    //        var estatusIds = estatusDict.Keys.ToList();

    //        var datosEnBD = await _context.TbPedidos
    //            .Where(p => estatusIds.Contains(p.Estatus))
    //            .GroupBy(p => p.Estatus)
    //            .Select(g => new
    //            {
    //                Estatus = g.Key,
    //                Total = g.Count()
    //            })
    //            .ToListAsync();

    //        var resultado = estatusDict.Select(e => new PedidosEstatusDto
    //        {
    //            EstatusNombre = e.Value,
    //            Total = datosEnBD.FirstOrDefault(x => x.Estatus == e.Key)?.Total ?? 0
    //        }).ToList();

    //        return resultado;
    //    }

    //    public async Task<List<ProductoMasVendidoDto>> ObtenerProductosMasVendidosAsync()
    //    {
    //        var resultado = await _context.TbCotizacionDetalles
    //            .Include(cd => cd.Cotizacion)
    //                .ThenInclude(c => c.Producto)
    //            .Where(cd => cd.Cotizacion.Producto != null && cd.Cotizacion.Producto.Estatus == 1)
    //            .GroupBy(cd => new { cd.Cotizacion.ProductoId, cd.Cotizacion.Producto.Nombre })
    //            .Select(g => new ProductoMasVendidoDto
    //            {
    //                NombreProducto = g.Key.Nombre,
    //                TotalVendido = g.Sum(cd => cd.Cantidad * cd.PrecioPromedio)
    //            })
    //            .OrderByDescending(p => p.TotalVendido)
    //            .ToListAsync();

    //        return resultado;
    //    }


    //    public async Task<List<ProductoOpinionDto>> ObtenerProductosMejorCalificadosAsync()
    //    {
    //        var resultado = await _context.TbOpiniones
    //            .Where(o => o.Estatus == 1 && o.ProductoId != null && o.Calificacion != null)
    //            .GroupBy(o => o.Producto)
    //            .Select(g => new ProductoOpinionDto
    //            {
    //                NombreProducto = g.Key.Nombre ?? "Sin nombre",
    //                PromedioCalificacion = Math.Round(g.Average(x => x.Calificacion!.Value), 2),
    //                TotalOpiniones = g.Count()
    //            })
    //            .OrderByDescending(x => x.PromedioCalificacion)
    //            .ToListAsync();

    //        return resultado;
    //    }

    //    public async Task<List<DistribucionOpinionDto>> ObtenerDistribucionOpinionesAsync()
    //    {
    //        var datos = await _context.TbOpiniones
    //            .Where(o => o.Estatus == 1 && o.Calificacion != null)
    //            .GroupBy(o => o.Calificacion!.Value)
    //            .Select(g => new DistribucionOpinionDto
    //            {
    //                Calificacion = g.Key,
    //                Total = g.Count()
    //            })
    //            .ToListAsync();

    //        // Asegurar que todas las calificaciones del 1 al 5 estén presentes
    //        var resultadoFinal = Enumerable.Range(1, 5)
    //            .Select(i => new DistribucionOpinionDto
    //            {
    //                Calificacion = i,
    //                Total = datos.FirstOrDefault(x => x.Calificacion == i)?.Total ?? 0
    //            })
    //            .ToList();

    //        return resultadoFinal;
    //    }


    //}
}
