using System.Linq;
using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Productos;
using IntelliSoftAPIV2.Dtos.Unidades;
using Microsoft.Data.SqlClient;
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
                .Where(p => p.Estatus == 1)
                .ToListAsync();

            return productos.Select(p =>
            {
                decimal precioBase = p.TbProductoInsumos.Sum(pi =>
                {
                    var inventario = pi.Insumo?.TbInventarioInsumos
                        .Where(ii => ii.Promedio != null)
                        .OrderByDescending(ii => ii.Fecha)
                        .FirstOrDefault();

                    decimal precioPromedio = inventario?.Promedio ?? inventario?.Costo ?? 0;
                    return (pi.Cantidad ?? 0) * precioPromedio;
                });

                var ganancia = (precioBase * (p.PorcentajeGanancia / 100));
                var riesgo = (precioBase * (p.PorcentajeRiesgo / 100));

                return new ProductoResumenDto
                {
                    IdProductos = p.IdProductos,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    HectareaBase = p.HectareaBase,
                    PrecioActual = precioBase,
                    PrecioCosto = precioBase,
                    Ganancia = ganancia,
                    PorcentajeGanancia = p.PorcentajeGanancia,
                    PorcentajeRiesgo = p.PorcentajeRiesgo,

                    PrecioConGanancia = precioBase + ganancia,
                    PrecioConRiesgo = precioBase + ganancia + riesgo
                };
            })
            .ToList();
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
            decimal precioBase = 0;

            foreach (var pi in producto.TbProductoInsumos)
            {
                var ultimoInventario = await _context.TbInventarioInsumos
                    .Where(i => i.InsumoId == pi.InsumoId && i.Promedio != null)
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

                precioBase += cantidad * precioPromedio;
            }

            var conGanancia = precioBase * (1 + (producto.PorcentajeGanancia / 100));
            var conRiesgo = conGanancia * (1 + (producto.PorcentajeRiesgo / 100));

            return new ProductoDetalleDto
            {
                IdProductos = producto.IdProductos,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                HectareaBase = producto.HectareaBase,
                PorcentajeGanancia = producto.PorcentajeGanancia,
                PorcentajeRiesgo = producto.PorcentajeRiesgo,
                PrecioActual = Math.Round(precioBase, 2),
                PrecioCosto = Math.Round(precioBase, 2),
                PrecioConGanancia = Math.Round(conGanancia, 2),
                PrecioConRiesgo = Math.Round(conRiesgo, 2),
                Insumos = insumoDetalles
            };
        }

        public async Task<ServiceResult<string>> Create(ProductoCreateDto dto)
        {
            var producto = new TbProducto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                HectareaBase = dto.HectareaBase,
                PorcentajeGanancia = dto.PorcentajeGanancia,
                PorcentajeRiesgo = dto.PorcentajeRiesgo
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
            producto.PorcentajeGanancia = dto.PorcentajeGanancia;
            producto.PorcentajeRiesgo = dto.PorcentajeRiesgo;

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

        public async Task<ProductoDocumentosDto?> ObtenerProductoDocumentosAsync(int idProducto)
        {
            var producto = await _context.TbProductos
                .FirstOrDefaultAsync(p => p.IdProductos == idProducto && p.Estatus == 1);

            if (producto == null)
                return null;

            var documentos = await _context.TbDocumentos
                .Where(d => d.IdProductos == idProducto)
                .Select(d => new DocumentoDto
                {
                    IdDocumento = d.IdDocumento,
                    NombreDocumento = d.NombreDocumento,
                    Url = d.Url
                })
                .ToListAsync();

            return new ProductoDocumentosDto
            {
                IdProducto = producto.IdProductos,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Documentos = documentos
            };
        }


        public async Task<List<ProductoDocumentosDto>> ObtenerProductosDocumentosAsync(string userId)
        {
            var query = @"
        SELECT 
            p.id_productos AS IdProducto,
            p.nombre AS Nombre,
            p.descripcion AS Descripcion,
            d.idDocumento AS IdDocumento,
            d.nombre_columna AS NombreDocumento,
            d.url AS Url
        FROM [catalogos].[TB_Productos] p
        LEFT JOIN [catalogos].[TB_Documento] d ON p.id_productos = d.id_producto
        WHERE EXISTS (
            SELECT 1
            FROM [operaciones].[TB_Cotizaciones] c
            WHERE p.id_productos = c.producto_id 
            AND EXISTS (
                SELECT 1
                FROM [operaciones].[TB_Pedido] ped
                WHERE c.id_cotizaciones = ped.cotizacion_id 
                AND ped.estatus >= 2
            ) 
            AND c.cliente_id = @userId
        )
        ORDER BY p.id_productos";

            var productos = new List<ProductoDocumentosDto>();

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.Add(new SqlParameter("@userId", userId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var productosDict = new Dictionary<int, ProductoDocumentosDto>();

                        while (await reader.ReadAsync())
                        {
                            var productoId = reader.GetInt32(0);

                            if (!productosDict.TryGetValue(productoId, out var productoDto))
                            {
                                productoDto = new ProductoDocumentosDto
                                {
                                    IdProducto = productoId,
                                    Nombre = reader.GetString(1),
                                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Documentos = new List<DocumentoDto>()
                                };
                                productosDict.Add(productoId, productoDto);
                                productos.Add(productoDto);
                            }

                            if (!reader.IsDBNull(3)) // Si hay documento
                            {
                                productoDto.Documentos.Add(new DocumentoDto
                                {
                                    IdDocumento = reader.GetInt32(3),
                                    NombreDocumento = reader.GetString(4),
                                    Url = reader.GetString(5)
                                });
                            }
                        }
                    }
                }
            }

            return productos;
        }


    }
}
