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
                .Select(c => new CotizacionResumenDto
                {
                    IdCotizacion = c.IdCotizaciones,
                    ClaveCotizacion = c.ClaveCotizacion,
                    FechaSolicitud = c.FechaSolicitud ?? DateTime.MinValue,
                    EstadoSolicitud = c.Estatus,
                    NombreCliente = c.Usuario.Nombre + " " + c.Usuario.Apellidos
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

        public async Task<bool> CrearCotizacion(CotizacionCreateDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(dto.UsuarioId);
            if (usuario == null) return false;

            var cotizacion = new TbCotizacion
            {
                ClaveCotizacion = GenerarClaveCotizacion(),
                UsuarioId = dto.UsuarioId,
                Hectareas = dto.Hectareas,
                FechaSolicitud = DateTime.UtcNow,
                Estatus = 1
            };

            _context.TbCotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            foreach (var detalle in dto.Detalles)
            {
                _context.TbCotizacionDetalles.Add(new TbCotizacionDetalle
                {
                    CotizacionId = cotizacion.IdCotizaciones,
                    InsumoId = detalle.InsumoId,
                    Cantidad = detalle.Cantidad,
                    PrecioPromedio = detalle.PrecioPromedio
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarEstadoCotizacion(CotizacionEstadoUpdateDto dto)
        {
            var cotizacion = await _context.TbCotizaciones.FindAsync(dto.IdCotizacion);
            if (cotizacion == null) return false;

            cotizacion.Estatus = dto.NuevoEstado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AceptarCotizacion(AceptarCotizacionDto dto)
        {
            var cotizacion = await _context.TbCotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.IdCotizaciones == dto.IdCotizacion);

            if (cotizacion == null || cotizacion.Estatus != 1) return false;

            // Crear pedido
            var pedido = new TbPedido
            {
                CotizacionId = cotizacion.IdCotizaciones,
                FechaPedido = DateTime.UtcNow,
                Estatus = 1
            };

            _context.TbPedidos.Add(pedido);
            cotizacion.Estatus = 2;

            // Cambiar rol si es anónimo
            var usuario = cotizacion.Usuario;
            var roles = await _userManager.GetRolesAsync(usuario);
            if (roles.Contains("anonimo"))
            {
                await _userManager.RemoveFromRoleAsync(usuario, "anonimo");
                if (!roles.Contains("cliente"))
                    await _userManager.AddToRoleAsync(usuario, "cliente");
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerarClaveCotizacion()
        {
            return "COT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
}
