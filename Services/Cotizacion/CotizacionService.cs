using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Cotizacion;
using IntelliSoftAPIV2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Cotizacion
{
    public class CotizacionService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CotizacionService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<CotizacionResumenDto>> GetCotizacionesResumen()
        {
            return await _context.TbCotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                .Select(c => new CotizacionResumenDto
                {
                    IdCotizacion = c.IdCotizaciones,
                    ClaveCotizacion = c.ClaveCotizacion,
                    FechaSolicitud = c.FechaSolicitud ?? DateTime.MinValue,
                    EstadoSolicitud = c.Estatus,
                    NombreCliente = c.Usuario.Nombre + " " + c.Usuario.Apellidos,
                    Total = c.Detalles.Sum(d => d.Cantidad * d.PrecioPromedio)
                })
                .ToListAsync();
        }

        public async Task<CotizacionDto?> GetCotizacionById(int id)
        {
            var cotizacion = await _context.TbCotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                .ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(c => c.IdCotizaciones == id);

            if (cotizacion == null) return null;

            return new CotizacionDto
            {
                IdCotizacion = cotizacion.IdCotizaciones,
                ClaveCotizacion = cotizacion.ClaveCotizacion,
                Hectareas = cotizacion.Hectareas,
                EstadoSolicitud = cotizacion.Estatus,
                FechaSolicitud = cotizacion.FechaSolicitud ?? DateTime.MinValue,
                UsuarioId = cotizacion.UsuarioId,
                Detalles = cotizacion.Detalles.Select(d => new CotizacionDetalleDto
                {
                    InsumoId = d.InsumoId,
                    NombreInsumo = d.Insumo.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioPromedio = d.PrecioPromedio
                }).ToList()
            };
        }

        public async Task<ServiceResult<string>> CrearCotizacion(CotizacionCreateDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(dto.UsuarioId);
            if (usuario == null)
                return ServiceResult<string>.Failure("Usuario no encontrado");

            // Obtener producto y su receta
            var producto = await _context.TbProductos
                .Include(p => p.TbProductoInsumos)
                .FirstOrDefaultAsync(p => p.IdProductos == dto.ProductoId);

            if (producto == null || producto.HectareaBase <= 0 || producto.TbProductoInsumos.Count == 0)
                return ServiceResult<string>.Failure("Producto inválido o sin receta");

            // Generar clave
            var clave = GenerarClaveCotizacion();

            // Crear cotización
            var cotizacion = new TbCotizacion
            {
                ClaveCotizacion = clave,
                UsuarioId = dto.UsuarioId,
                ProductoId = dto.ProductoId,
                Hectareas = dto.Hectareas,
                DetalleCotizacion = dto.DetalleCotizacion,
                FechaSolicitud = DateTime.UtcNow,
                Estatus = 1
            };

            _context.TbCotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            var factorEscala = dto.Hectareas / producto.HectareaBase;

            foreach (var receta in producto.TbProductoInsumos)
            {
                var insumoId = receta.InsumoId;
                var cantidadEscalada = receta.Cantidad * factorEscala;

                var inventario = await _context.TbInventarioInsumos
                    .Where(i => i.InsumoId == insumoId)
                    .OrderByDescending(i => i.Fecha)
                    .FirstOrDefaultAsync();

                var precioPromedio = inventario?.Promedio ?? 0;

                var detalle = new TbCotizacionDetalle
                {
                    CotizacionId = cotizacion.IdCotizaciones,
                    InsumoId = insumoId ?? 0,
                    Cantidad = Math.Round(cantidadEscalada ?? 0, 2),
                    PrecioPromedio = Math.Round(precioPromedio, 2)
                };

                _context.TbCotizacionDetalles.Add(detalle);
            }

            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess(clave, "Cotización registrada correctamente");
        }

        public async Task<ServiceResult<string>> ActualizarEstadoCotizacion(CotizacionEstadoUpdateDto dto)
        {
            var cotizacion = await _context.TbCotizaciones.FindAsync(dto.IdCotizacion);
            if (cotizacion == null)
                return ServiceResult<string>.Failure("Cotización no encontrada");

            cotizacion.Estatus = dto.NuevoEstado;
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess(
                data: "Estatus de la cotización actualizado correctamente",
                message: "Estatus de la cotización actualizado correctamente"
            );
        }

        public async Task<ServiceResult<string>> AceptarCotizacion(AceptarCotizacionDto dto)
        {
            var cotizacion = await _context.TbCotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.IdCotizaciones == dto.IdCotizacion);

            if (cotizacion == null || cotizacion.Estatus != 1)
                return ServiceResult<string>.Failure("Cotización no encontrada o ya procesada");

            var pedido = new TbPedido
            {
                CotizacionId = cotizacion.IdCotizaciones,
                FechaPedido = DateTime.UtcNow,
                Estatus = 1
            };

            _context.TbPedidos.Add(pedido);
            cotizacion.Estatus = 2;

            var usuario = cotizacion.Usuario;
            var roles = await _userManager.GetRolesAsync(usuario);
            if (roles.Contains("anonimo"))
            {
                await _userManager.RemoveFromRoleAsync(usuario, "anonimo");
                if (!roles.Contains("cliente"))
                    await _userManager.AddToRoleAsync(usuario, "cliente");
            }

            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess(
                data: "Cotización aceptada correctamente",
                message: "Cotización aceptada correctamente"
            );
        }

        private string GenerarClaveCotizacion()
        {
            return "COT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
}
