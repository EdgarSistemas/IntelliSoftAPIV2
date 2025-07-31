using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Compra;
using IntelliSoftAPIV2.Dtos.Proveedores;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Compra
{
    public class CompraService
    {
        private readonly AppDbContext _context;

        public CompraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<string>> CrearCompra(CompraCreateDto dto)
        {
            var compra = new TbCompra
            {
                ClaveCompra = $"C-{DateTime.Now.Ticks}",
                FechaCompra = DateTime.Now,
                Observacion = dto.Observacion,
                Estatus = 0, // creada
                ProveedorId = dto.ProveedorId
            };

            foreach (var detalle in dto.Detalles)
            {
                compra.TbCompraDetalles.Add(new TbCompraDetalle
                {
                    InsumoId = detalle.InsumoId,
                    Presentacion = detalle.Presentacion,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Cantidad = detalle.Cantidad
                });
            }
            
            _context.TbCompras.Add(compra);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess("Compra registrada correctamente");
        }

        public async Task<ServiceResult<string>> CancelarCompra(int id)
        {
            var compra = await _context.TbCompras
                .Include(c => c.TbInventarioInsumos)
                .FirstOrDefaultAsync(c => c.IdCompra == id);

            if (compra == null)
                return ServiceResult<string>.Failure("Compra no encontrada");

            if (compra.Estatus != 1)
            {
                // Si no fue inventariada, simplemente cambia el estatus
                compra.Estatus = 2;
                await _context.SaveChangesAsync();
                return ServiceResult<string>.CreateSuccess("Compra cancelada correctamente");
            }

            // Guarda insumos afectados antes de eliminar
            var insumosAfectados = compra.TbInventarioInsumos
                .Select(i => i.InsumoId)
                .Distinct()
                .ToList();

            // Elimina los registros de inventario de esta compra
            _context.TbInventarioInsumos.RemoveRange(compra.TbInventarioInsumos);
            compra.Estatus = 2; // Cancelada

            await _context.SaveChangesAsync();

            // Recalcula inventario de cada insumo afectado
            foreach (var insumoId in insumosAfectados)
            {
                await RecalcularInventarioInsumo(insumoId);
            }

            return ServiceResult<string>.CreateSuccess("Compra cancelada y existencias recalculadas correctamente");
        }

        private async Task RecalcularInventarioInsumo(int insumoId)
        {
            var inventarios = await _context.TbInventarioInsumos
                .Where(i => i.InsumoId == insumoId)
                .OrderBy(i => i.Fecha)
                .ToListAsync();

            decimal saldo = 0;
            int existencia = 0;

            foreach (var item in inventarios)
            {
                if (item.Entrada.HasValue)
                {
                    existencia += item.Entrada.Value;
                    saldo += item.Debe ?? 0;
                }

                if (item.Salida.HasValue)
                {
                    existencia -= item.Salida.Value;
                    saldo -= item.Haber ?? 0;
                }

                item.Existencias = existencia;
                item.Saldo = saldo;
                item.Promedio = existencia > 0 ? saldo / existencia : 0;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CompraResumenDto>> ListarCompras()
        {
            return await _context.TbCompras
                .Include(c => c.Proveedor)
                .Select(c => new CompraResumenDto
                {
                    IdCompra = c.IdCompra,
                    ClaveCompra = c.ClaveCompra,
                    FechaCompra = c.FechaCompra,
                    Observacion = c.Observacion,
                    Estatus = c.Estatus ?? 0,
                    Proveedor = new ProveedorResponseDto
                    {
                        IdProveedor = c.Proveedor.IdProveedor,
                        Nombre = c.Proveedor.Nombre,
                        Telefono = c.Proveedor.Telefono,
                        Contacto = c.Proveedor.Contacto,
                        CorreoElectronico = c.Proveedor.CorreoElectronico,
                        DescripcionServicio = c.Proveedor.DescripcionServicio,
                        Estatus = c.Proveedor.Estatus
                    },
                    Total = c.TbCompraDetalles.Sum(cd => cd.Cantidad * cd.PrecioUnitario)
                }).ToListAsync();
        }

        public async Task<CompraDetalleDto?> ObtenerCompraPorId(int id)
        {
            var compra = await _context.TbCompras
                .Include(c => c.Proveedor)
                .Include(c => c.TbCompraDetalles)
                .ThenInclude(cd => cd.Insumo)
                .ThenInclude(i => i.Unidad)
                .FirstOrDefaultAsync(c => c.IdCompra == id);

            if (compra == null) return null;

            return new CompraDetalleDto
            {
                IdCompra = compra.IdCompra,
                ClaveCompra = compra.ClaveCompra,
                FechaCompra = compra.FechaCompra,
                Estatus = compra.Estatus,
                Observacion = compra.Observacion,
                Proveedor = new ProveedorResponseDto
                {
                    IdProveedor = compra.Proveedor.IdProveedor,
                    Nombre = compra.Proveedor.Nombre,
                    Telefono = compra.Proveedor.Telefono,
                    Contacto = compra.Proveedor.Contacto,
                    CorreoElectronico = compra.Proveedor.CorreoElectronico,
                    DescripcionServicio = compra.Proveedor.DescripcionServicio,
                    Estatus = compra.Proveedor.Estatus
                },
                Detalles = compra.TbCompraDetalles.Select(cd => new CompraDetalleItemDto
                {
                    IdCompraDetalle = cd.IdCompraDetalle,
                    InsumoId = cd.InsumoId,
                    InsumoNombre = cd.Insumo.Nombre,
                    Presentacion = cd.Presentacion,
                    Cantidad = cd.Cantidad,
                    PrecioUnitario = cd.PrecioUnitario,
                    Unidad = new()
                    {
                        IdUnidad = cd.Insumo.Unidad.IdUnidad,
                        Nombre = cd.Insumo.Unidad.Nombre,
                        Simbolo = cd.Insumo.Unidad.Simbolo,
                        Descripcion = cd.Insumo.Unidad.Descripcion,
                        Estatus = cd.Insumo.Unidad.Estatus ?? 1
                    }
                }).ToList()
            };
        }

        public async Task<List<CompraResumenDto>> ListarComprasNoInventariadas()
        {
            return await _context.TbCompras
                .Where(c => c.Estatus == 0)
                .Include(c => c.Proveedor)
                .Select(c => new CompraResumenDto
                {
                    IdCompra = c.IdCompra,
                    ClaveCompra = c.ClaveCompra,
                    FechaCompra = c.FechaCompra,
                    Observacion = c.Observacion,
                    Estatus = c.Estatus,
                    Proveedor = new ProveedorResponseDto
                    {
                        IdProveedor = c.Proveedor.IdProveedor,
                        Nombre = c.Proveedor.Nombre,
                        Telefono = c.Proveedor.Telefono,
                        Contacto = c.Proveedor.Contacto,
                        CorreoElectronico = c.Proveedor.CorreoElectronico,
                        DescripcionServicio = c.Proveedor.DescripcionServicio,
                        Estatus = c.Proveedor.Estatus
                    },
                    Total = c.TbCompraDetalles.Sum(cd => cd.Cantidad * cd.PrecioUnitario)
                }).ToListAsync();
        }

        public async Task<List<CompraDetalleItemDto>> ObtenerDetalleInventarioPorId(int compraId)
        {
            return await _context.TbCompraDetalles
                .Where(d => d.CompraId == compraId)
                .Include(cd => cd.Insumo)
                .ThenInclude(i => i.Unidad)
                .Select(cd => new CompraDetalleItemDto
                {
                    IdCompraDetalle = cd.IdCompraDetalle,
                    InsumoId = cd.InsumoId,
                    InsumoNombre = cd.Insumo.Nombre,
                    Presentacion = cd.Presentacion,
                    Cantidad = cd.Cantidad,
                    PrecioUnitario = cd.PrecioUnitario,
                    Unidad = new()
                    {
                        IdUnidad = cd.Insumo.Unidad.IdUnidad,
                        Nombre = cd.Insumo.Unidad.Nombre,
                        Simbolo = cd.Insumo.Unidad.Simbolo,
                        Descripcion = cd.Insumo.Unidad.Descripcion,
                        Estatus = cd.Insumo.Unidad.Estatus ?? 1
                    }
                }).ToListAsync();
        }

        public async Task<ServiceResult<string>> InventariarCompra(InventarioCreateDto dto)
        {
            var compra = await _context.TbCompras.FindAsync(dto.CompraId);
            if (compra == null)
                return ServiceResult<string>.Failure("Compra no encontrada");

            foreach (var item in dto.InsumosInventariados)
            {
                var ultimo = await _context.TbInventarioInsumos
                    .Where(i => i.InsumoId == item.InsumoId)
                    .OrderByDescending(i => i.Fecha)
                    .FirstOrDefaultAsync();

                var existenciaAnterior = ultimo?.Existencias ?? 0;
                var saldoAnterior = ultimo?.Saldo ?? 0;

                var nuevoSaldo = saldoAnterior + (item.CostoUnitario * item.CantidadUnidad);
                var nuevaExistencia = existenciaAnterior + item.CantidadUnidad;
                var nuevoPromedio = nuevaExistencia > 0 ? nuevoSaldo / nuevaExistencia : 0;

                _context.TbInventarioInsumos.Add(new TbInventarioInsumo
                {
                    Fecha = DateTime.Now,
                    Entrada = item.CantidadUnidad,
                    InsumoId = item.InsumoId,
                    Existencias = nuevaExistencia,
                    Costo = item.CostoUnitario,
                    Promedio = nuevoPromedio,
                    Debe = item.CostoUnitario * item.CantidadUnidad,
                    Haber = 0,
                    Saldo = nuevoSaldo,
                    CompraId = dto.CompraId
                });
            }

            compra.Estatus = 1; // inventariada
            await _context.SaveChangesAsync();
            return ServiceResult<string>.CreateSuccess("Compra inventariada correctamente");
        }
    }
}
