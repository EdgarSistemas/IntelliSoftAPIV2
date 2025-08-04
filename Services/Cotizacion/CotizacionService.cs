using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Configuration;
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
        private readonly EmailService _emailService;

        public CotizacionService(AppDbContext context, UserManager<ApplicationUser> userManager, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<List<CotizacionResumenDto>> GetCotizacionesResumen()
        {
            var cotizaciones = await _context.TbCotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                .Select(c => new
                {
                    c.IdCotizaciones,
                    c.ClaveCotizacion,
                    c.FechaSolicitud,
                    c.Estatus,
                    c.Hectareas,
                    ClienteNombre = c.Usuario.Nombre + " " + c.Usuario.Apellidos,
                    PorcentajeGanancia = c.PorcentajeGanancia,
                    PorcentajeRiesgo = c.PorcentajeRiesgo,
                    AplicaRiesgo = c.AplicaRiesgo,
                    Subtotal = c.Detalles.Sum(d => d.Cantidad * d.PrecioPromedio)
                })
                .OrderByDescending(c => c.FechaSolicitud)
                .ToListAsync();

            return cotizaciones.Select(c =>
            {
                var basePrice = Math.Round(c.Subtotal, 2);
                var conGanancia = Math.Round(basePrice * (1 + c.PorcentajeGanancia / 100), 2);
                var conRiesgo = c.AplicaRiesgo == 1
                    ? Math.Round(conGanancia * (1 + c.PorcentajeRiesgo / 100), 2)
                    : conGanancia;

                return new CotizacionResumenDto
                {
                    IdCotizacion = c.IdCotizaciones,
                    ClaveCotizacion = c.ClaveCotizacion,
                    FechaSolicitud = c.FechaSolicitud ?? DateTime.MinValue,
                    EstadoSolicitud = c.Estatus,
                    Hectareas = c.Hectareas,
                    NombreCliente = c.ClienteNombre,
                    PrecioBase = basePrice,
                    PrecioConGanancia = conGanancia,
                    PrecioConRiesgo = conRiesgo,
                    Total = conRiesgo
                };
            }).ToList();
        }

        public async Task<CotizacionDto?> GetCotizacionById(int id)
        {
            var cotizacion = await _context.TbCotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Detalles)
                .ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(c => c.IdCotizaciones == id);

            if (cotizacion == null) return null;

            var detallesDto = cotizacion.Detalles.Select(d => new CotizacionDetalleDto
            {
                InsumoId = d.InsumoId,
                NombreInsumo = d.Insumo.Nombre,
                Cantidad = d.Cantidad,
                PrecioPromedio = d.PrecioPromedio
            }).ToList();

            var precioBase = detallesDto.Sum(d => d.Subtotal);

            var porcentajeGanancia = cotizacion.PorcentajeGanancia;
            var porcentajeRiesgo = cotizacion.PorcentajeRiesgo;
            var aplicaRiesgo = cotizacion.AplicaRiesgo == 1;

            var precioConGanancia = Math.Round(precioBase * (1 + porcentajeGanancia / 100), 2);
            var precioConRiesgo = aplicaRiesgo
                ? Math.Round(precioConGanancia * (1 + porcentajeRiesgo / 100), 2)
                : precioConGanancia;

            return new CotizacionDto
            {
                IdCotizacion = cotizacion.IdCotizaciones,
                ClaveCotizacion = cotizacion.ClaveCotizacion,
                ProductoId = cotizacion.ProductoId ?? 0,
                UsuarioId = cotizacion.UsuarioId,
                Hectareas = cotizacion.Hectareas,
                EstadoSolicitud = cotizacion.Estatus,
                FechaSolicitud = cotizacion.FechaSolicitud ?? DateTime.MinValue,
                Detalles = detallesDto,
                PrecioBase = Math.Round(precioBase, 2),
                PrecioConGanancia = precioConGanancia,
                PrecioConRiesgo = precioConRiesgo
            };
        }

        public async Task<ServiceResult<string>> CrearCotizacion(CotizacionCreateDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(dto.UsuarioId);
            if (usuario == null)
                return ServiceResult<string>.Failure("Usuario no encontrado");

            var producto = await _context.TbProductos
                .Include(p => p.TbProductoInsumos)
                .FirstOrDefaultAsync(p => p.IdProductos == dto.ProductoId);

            if (producto == null || producto.HectareaBase <= 0 || producto.TbProductoInsumos.Count == 0)
                return ServiceResult<string>.Failure("Producto inválido o sin receta");

            var porcentajeGanancia = producto.PorcentajeGanancia;
            var porcentajeRiesgo = producto.PorcentajeRiesgo;

            var clave = GenerarClaveCotizacion();

            var cotizacion = new TbCotizacion
            {
                ClaveCotizacion = clave,
                UsuarioId = dto.UsuarioId,
                ProductoId = dto.ProductoId,
                Hectareas = dto.Hectareas,
                DetalleCotizacion = dto.DetalleCotizacion,
                FechaSolicitud = DateTime.UtcNow,
                Estatus = 1,
                PorcentajeGanancia = porcentajeGanancia,
                PorcentajeRiesgo = porcentajeRiesgo,
                AplicaRiesgo = 1
            };

            _context.TbCotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            var factorEscala = dto.Hectareas / producto.HectareaBase;

            foreach (var receta in producto.TbProductoInsumos)
            {
                var insumoId = receta.InsumoId;
                var cantidadEscalada = receta.Cantidad * factorEscala;

                var inventario = await _context.TbInventarioInsumos
                    .Where(i => i.InsumoId == insumoId && i.Promedio != null)
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

            // Si el usuario era anónimo, lo promovemos y enviamos las credenciales
            if (roles.Contains("anonimo"))
            {
                await _userManager.RemoveFromRoleAsync(usuario, "anonimo");

                if (!roles.Contains("cliente"))
                    await _userManager.AddToRoleAsync(usuario, "cliente");

                if (!string.IsNullOrEmpty(usuario.ContrasenaGenerada))
                {
                    string cuerpoHtml = $@"
                <h3>¡Tu cotización ha sido aceptada!</h3>
                <p>Tu acceso al sistema está listo:</p>
                <ul>
                    <li><b>Email:</b> {usuario.Email}</li>
                    <li><b>Contraseña:</b> {usuario.ContrasenaGenerada}</li>
                </ul>
                <p>Por seguridad, te recomendamos cambiar la contraseña después de iniciar sesión.</p>";

                    try
                    {
                        await _emailService.EnviarCorreoAsync(usuario.Email, "Acceso a AquaGrow", cuerpoHtml);
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<string>.Failure($"Error al enviar correo: {ex.Message}");
                    }

                    usuario.ContrasenaGenerada = null;
                }
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
