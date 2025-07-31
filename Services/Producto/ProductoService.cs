using System.Linq;
using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Productos;
using IntelliSoftAPIV2.Dtos.Unidades;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Producto
{
    public class ProductoService
    {
        private readonly AppDbContext _context;

        public ProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductoResumenDto>> GetAll()
        {
            var productos = await _context.TbProductos
                .Include(p => p.TbProductoInsumos)
                    .ThenInclude(pi => pi.Insumo)
                    .ThenInclude(i => i.TbInventarioInsumos)
                .ToListAsync();

            return productos.Select(p => new ProductoResumenDto
            {
                IdProductos = p.IdProductos,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                HectareaBase = p.HectareaBase,
                PrecioActual = p.TbProductoInsumos.Sum(pi =>
                {
                    var insumo = pi.Insumo;
                    var promedio = insumo?.TbInventarioInsumos
                        .OrderByDescending(ii => ii.Fecha)
                        .Select(ii => ii.Promedio)
                        .FirstOrDefault() ?? 0;

                    return (pi.Cantidad ?? 0) * promedio;
                })
            }).ToList();
        }

        public async Task<ProductoDetalleDto?> GetById(int id)
        {
            var producto = await _context.TbProductos
                .Include(p => p.TbProductoInsumos)
                .ThenInclude(pi => pi.Insumo)
                .ThenInclude(i => i.Unidad)
                .FirstOrDefaultAsync(p => p.IdProductos == id);

            if (producto == null)
                return null;

            var insumoDetalles = new List<ProductoInsumoDetalleDto>();
            decimal precioTotal = 0;

            foreach (var pi in producto.TbProductoInsumos)
            {
                var ultimoInventario = await _context.TbInventarioInsumos
                    .Where(i => i.InsumoId == pi.InsumoId)
                    .OrderByDescending(i => i.Fecha)
                    .FirstOrDefaultAsync();

                var precioPromedio = ultimoInventario?.Promedio ?? 0;
                var cantidad = pi.Cantidad ?? 0;

                insumoDetalles.Add(new ProductoInsumoDetalleDto
                {
                    InsumoId = pi.InsumoId ?? 0,
                    Nombre = pi.Insumo?.Nombre ?? "",
                    Cantidad = cantidad,
                    PrecioPromedio = precioPromedio,
                    Unidad = new UnidadDto
                    {
                        IdUnidad = pi.Insumo!.Unidad.IdUnidad,
                        Nombre = pi.Insumo.Unidad.Nombre,
                        Simbolo = pi.Insumo.Unidad.Simbolo,
                        Descripcion = pi.Insumo.Unidad.Descripcion,
                        Estatus = pi.Insumo.Unidad.Estatus
                    }
                });

                precioTotal += cantidad * precioPromedio;
            }

            return new ProductoDetalleDto
            {
                IdProductos = producto.IdProductos,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioActual = precioTotal,
                HectareaBase = producto.HectareaBase,
                Insumos = insumoDetalles
            };
        }

        public async Task<ServiceResult<string>> Create(ProductoCreateDto dto)
        {
            var producto = new TbProducto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                HectareaBase = dto.HectareaBase
            };

            foreach (var insumo in dto.Insumos)
            {
                producto.TbProductoInsumos.Add(new TbProductoInsumo
                {
                    InsumoId = insumo.InsumoId,
                    Cantidad = insumo.Cantidad
                });
            }

            _context.TbProductos.Add(producto);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess(
                data: "Producto creado correctamente",
                message: "Producto creado correctamente"
            );
        }

        public async Task<ServiceResult<string>> Update(int id, ProductoCreateDto dto)
        {
            var producto = await _context.TbProductos
        .Include(p => p.TbProductoInsumos)
        .FirstOrDefaultAsync(p => p.IdProductos == id);

            if (producto == null)
                return ServiceResult<string>.Failure("Producto no encontrado");

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.HectareaBase = dto.HectareaBase;

            // Validar que los insumos no han sido agregados ni eliminados
            var idsExistentes = producto.TbProductoInsumos.Select(x => x.InsumoId!.Value).OrderBy(x => x).ToList();
            var idsDto = dto.Insumos.Select(x => x.InsumoId).OrderBy(x => x).ToList();

            if (!idsExistentes.SequenceEqual(idsDto))
                return ServiceResult<string>.Failure("No se permite agregar o eliminar insumos. Solo se pueden modificar cantidades.");

            // Actualizar cantidades
            foreach (var insumoExistente in producto.TbProductoInsumos)
            {
                var dtoInsumo = dto.Insumos.FirstOrDefault(x => x.InsumoId == insumoExistente.InsumoId);
                if (dtoInsumo != null)
                {
                    insumoExistente.Cantidad = dtoInsumo.Cantidad;
                }
            }

            await _context.SaveChangesAsync();
            return ServiceResult<string>.CreateSuccess(
                data: "Producto actualizado correctamente",
                message: "Producto actualizado correctamente"
            );
        }

        public async Task<ServiceResult<string>> Delete(int id)
        {
            var producto = await _context.TbProductos.FindAsync(id);

            if (producto == null)
                return ServiceResult<string>.Failure("Producto no encontrado");
            producto.Estatus = 0;
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess(
                data: "Producto eliminado correctamente",
                message: "Producto eliminado correctamente"
            );
        }
    }
}
