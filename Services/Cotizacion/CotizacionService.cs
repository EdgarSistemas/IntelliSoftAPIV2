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
        private readonly IEmailService _emailService;
        private readonly PdfGeneratorService _pdf;
        private readonly ILogger<CotizacionService> _logger;

        public CotizacionService(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            PdfGeneratorService pdfGeneratorService,
            ILogger<CotizacionService> logger)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _pdf = pdfGeneratorService;
            _logger = logger;
        }

        // ============================
        // Crear (header + N partidas)
        // ============================
        public async Task<ServiceResult<string>> CrearCotizacion(CotizacionCreateDto dto)
        {
            if (dto.Partidas == null || dto.Partidas.Count == 0)
                return ServiceResult<string>.Failure("Debes enviar al menos un producto (partida).");

            // Validación usuario (si mandas UsuarioId, debe existir)
            var usuario = dto.UsuarioId != null
                ? await _userManager.FindByIdAsync(dto.UsuarioId)
                : null;

            if (dto.UsuarioId != null && usuario == null)
                return ServiceResult<string>.Failure("Usuario no encontrado.");

            // Validaciones básicas de partidas
            foreach (var p in dto.Partidas)
            {
                if (p.Hectareas <= 0)
                    return ServiceResult<string>.Failure("Las hectáreas deben ser mayores a cero.");
            }

            var clave = GenerarClaveCotizacion();

            using var trx = await _context.Database.BeginTransactionAsync();
            try
            {
                var header = new TbCotizacion
                {
                    ClaveCotizacion = clave,
                    UsuarioId = dto.UsuarioId,
                    DetalleCotizacion = dto.DetalleCotizacion,
                    FechaSolicitud = DateTime.UtcNow,
                    Estatus = 1
                };

                _context.TbCotizaciones.Add(header);
                await _context.SaveChangesAsync();

                foreach (var p in dto.Partidas)
                {
                    var producto = await _context.TbProductos
                        .Include(x => x.TbProductoInsumos)
                        .FirstOrDefaultAsync(x => x.IdProductos == p.ProductoId);

                    if (producto == null)
                        return ServiceResult<string>.Failure($"Producto {p.ProductoId} no encontrado.");

                    if (producto.HectareaBase == null || producto.HectareaBase <= 0 || producto.TbProductoInsumos.Count == 0)
                        return ServiceResult<string>.Failure($"Producto '{producto.Nombre}' inválido o sin receta.");

                    var partida = new TbCotizacionProducto
                    {
                        CotizacionId = header.IdCotizaciones,
                        ProductoId = producto.IdProductos,
                        Hectareas = p.Hectareas,
                        // SIEMPRE tomar del producto; ya no vienen en el payload
                        PorcentajeGanancia = producto.PorcentajeGanancia,
                        PorcentajeRiesgo = producto.PorcentajeRiesgo,
                        AplicaRiesgo = 1
                    };
                    _context.TbCotizacionProductos.Add(partida);
                    await _context.SaveChangesAsync();

                    var factor = p.Hectareas / (producto.HectareaBase ?? 1m);

                    foreach (var receta in producto.TbProductoInsumos)
                    {
                        // Precio promedio del inventario más reciente (si no hay, 0)
                        var precioPromedio = await _context.TbInventarioInsumos
                            .Where(i => i.InsumoId == receta.InsumoId && i.Promedio != null)
                            .OrderByDescending(i => i.Fecha)
                            .Select(i => i.Promedio)
                            .FirstOrDefaultAsync() ?? 0m;

                        // si TbProductoInsumo.Cantidad es nullable, usa (receta.Cantidad ?? 0m)
                        var cantidadEscalada = (receta.Cantidad ?? 0m) * factor;

                        var det = new TbCotizacionProductoDetalle
                        {
                            CotizacionProductoId = partida.IdCotizacionProducto,
                            InsumoId = receta.InsumoId ?? 0,
                            Cantidad = decimal.Round(cantidadEscalada, 2),
                            PrecioPromedio = decimal.Round(precioPromedio, 2)
                        };
                        _context.TbCotizacionProductoDetalles.Add(det);
                    }

                    await _context.SaveChangesAsync();
                }

                await trx.CommitAsync();
                return ServiceResult<string>.CreateSuccess(clave, "Cotización creada correctamente.");
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                _logger.LogError(ex, "Error creando cotización.");
                return ServiceResult<string>.Failure("Error al crear la cotización.");
            }
        }

        // ============================
        // Resumen por PARTIDA
        // ============================
        public async Task<List<CotizacionConPartidasDto>> GetCotizacionesResumen()
        {
            var headers = await _context.TbCotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(c => c.CotizacionProductos)
                    .ThenInclude(cp => cp.Detalles)
                        .ThenInclude(d => d.Insumo)
                .OrderByDescending(c => c.FechaSolicitud)
                .ToListAsync();

            var list = new List<CotizacionConPartidasDto>();

            foreach (var c in headers)
            {
                var dto = new CotizacionConPartidasDto
                {
                    IdCotizacion = c.IdCotizaciones,
                    ClaveCotizacion = c.ClaveCotizacion,
                    UsuarioId = c.UsuarioId ?? "",
                    Estatus = c.Estatus,
                    FechaSolicitud = c.FechaSolicitud,
                    Partidas = new List<CotizacionProductoResumenDto>()
                };

                decimal totalCot = 0m;

                foreach (var cp in c.CotizacionProductos)
                {
                    var precioBase = cp.Detalles.Sum(d => d.Cantidad * d.PrecioPromedio);
                    precioBase = decimal.Round(precioBase, 2);

                    var ganancia = decimal.Round(precioBase * (cp.PorcentajeGanancia / 100m), 2);
                    var conGan = decimal.Round(precioBase * (1 + cp.PorcentajeGanancia / 100m), 2);
                    var conRiesgo = cp.AplicaRiesgo == 1
                                        ? decimal.Round(conGan * (1 + cp.PorcentajeRiesgo / 100m), 2)
                                        : conGan;

                    dto.Partidas.Add(new CotizacionProductoResumenDto
                    {
                        IdCotizacion = c.IdCotizaciones,
                        ClaveCotizacion = c.ClaveCotizacion,
                        FechaSolicitud = c.FechaSolicitud,
                        Estatus = c.Estatus,

                        CotizacionProductoId = cp.IdCotizacionProducto,
                        ProductoId = cp.ProductoId,
                        NombreProducto = cp.Producto?.Nombre,
                        Hectareas = cp.Hectareas,
                        NombreCliente = c.Usuario != null ? $"{c.Usuario.Nombre} {c.Usuario.Apellidos}" : null,

                        PrecioBase = precioBase,
                        Ganancia = ganancia,
                        PrecioConGanancia = conGan,
                        PrecioConRiesgo = conRiesgo,
                        Total = conGan
                    });

                    // Decide qué sumar como "TotalCotizacion".
                    // Aquí sumo PrecioConGanancia (igual que "Total" por partida).
                    totalCot += conGan;
                }

                dto.TotalCotizacion = decimal.Round(totalCot, 2);
                list.Add(dto);
            }

            return list;
        }

        // ============================
        // Obtener por ID (consolida partidas)
        // ============================
        public async Task<CotizacionFullDto?> GetCotizacionById(int id)
        {
            var c = await _context.TbCotizaciones
                .Include(x => x.Usuario)
                .Include(x => x.CotizacionProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(x => x.CotizacionProductos)
                    .ThenInclude(cp => cp.Detalles)
                        .ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(x => x.IdCotizaciones == id);

            if (c == null) return null;

            var dto = new CotizacionFullDto
            {
                IdCotizacion = c.IdCotizaciones,
                ClaveCotizacion = c.ClaveCotizacion,
                UsuarioId = c.UsuarioId,
                NombreCliente = c.Usuario != null ? $"{c.Usuario.Nombre} {c.Usuario.Apellidos}" : null,
                Estatus = c.Estatus,
                FechaSolicitud = c.FechaSolicitud,
                DetalleCotizacion = c.DetalleCotizacion
            };

            foreach (var cp in c.CotizacionProductos)
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

                dto.Partidas.Add(new CotizacionPartidaDto
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

            dto.TotalPrecioBase = dto.Partidas.Sum(p => p.PrecioBase);
            dto.TotalGanancia = dto.Partidas.Sum(p => p.Ganancia);
            dto.TotalPrecioConGanancia = dto.Partidas.Sum(p => p.PrecioConGanancia);
            dto.TotalPrecioConRiesgo = dto.Partidas.Sum(p => p.PrecioConRiesgo);
            dto.Total = dto.Partidas.Sum(p => p.Total);

            return dto;
        }

        // ============================
        // Cambiar estado header
        // ============================
        public async Task<ServiceResult<string>> ActualizarEstadoCotizacion(CotizacionEstadoUpdateDto dto)
        {
            var c = await _context.TbCotizaciones.FindAsync(dto.IdCotizacion);
            if (c == null) return ServiceResult<string>.Failure("Cotización no encontrada.");

            c.Estatus = dto.NuevoEstado;
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess(
                "OK",
                "Estatus de la cotización actualizado correctamente");
        }

        // ============================
        // Aceptar cotización (siempre manda email con PDF)
        // ============================
        public async Task<ServiceResult<string>> AceptarCotizacion(AceptarCotizacionDto dto)
        {
            var c = await _context.TbCotizaciones
                .Include(x => x.Usuario)
                .Include(x => x.CotizacionProductos).ThenInclude(cp => cp.Producto)
                .Include(x => x.CotizacionProductos).ThenInclude(cp => cp.Detalles).ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(x => x.IdCotizaciones == dto.IdCotizacion);

            if (c == null || c.Estatus != 1)
                return ServiceResult<string>.Failure("Cotización no encontrada o ya procesada.");

            if (c.Usuario == null)
                return ServiceResult<string>.Failure("La cotización no tiene un usuario asociado.");

            using var trx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1) Crear pedido
                var pedido = new TbPedido
                {
                    CotizacionId = c.IdCotizaciones,
                    FechaPedido = DateTime.UtcNow,
                    Estatus = 1
                };
                _context.TbPedidos.Add(pedido);

                // 2) Cambiar estado de la cotización
                c.Estatus = 2;

                // 3) Promover rol si era 'anonimo'
                var usuario = c.Usuario;
                var roles = await _userManager.GetRolesAsync(usuario);
                var eraAnonimo = roles.Contains("anonimo");

                if (eraAnonimo)
                {
                    await _userManager.RemoveFromRoleAsync(usuario, "anonimo");
                    await _userManager.AddToRoleAsync(usuario, "cliente");
                }

                // 4) Construir DTO y PDF (versión para cliente)
                var dtoFull = await GetCotizacionById(c.IdCotizaciones);
                if (dtoFull == null)
                    return ServiceResult<string>.Failure("No se pudo construir la cotización para PDF.");

                var pdfBytes = _pdf.GenerarPdfCotizacionConPartidas(dtoFull);

                // 5) Email: credenciales SOLO si era anónimo y hay ContrasenaGenerada
                var incluirCredenciales = eraAnonimo && !string.IsNullOrEmpty(usuario.ContrasenaGenerada);

                var cuerpo = @"
                    <h3>¡Tu cotización ha sido aceptada!</h3>
                    <p>Adjuntamos el PDF con el detalle por producto.</p>";

                if (incluirCredenciales)
                {
                    cuerpo += $@"
                    <p>Tus credenciales provisionales:</p>
                    <ul>
                        <li><b>Email:</b> {usuario.Email}</li>
                        <li><b>Contraseña:</b> {usuario.ContrasenaGenerada}</li>
                    </ul>
                    <p>Por seguridad, cambia la contraseña después de iniciar sesión.</p>";
                }

                await _emailService.EnviarCorreoAsync(
                    destinatario: usuario.Email ?? "",
                    asunto: $"Cotización aceptada - {c.ClaveCotizacion}",
                    cuerpoHtml: cuerpo,
                    archivoAdjunto: pdfBytes,
                    nombreArchivo: $"Cotizacion-{c.ClaveCotizacion}.pdf"
                );

                // 6) Limpiar contraseña provisional si se envió
                if (incluirCredenciales)
                    usuario.ContrasenaGenerada = null;

                await _context.SaveChangesAsync();
                await trx.CommitAsync();

                return ServiceResult<string>.CreateSuccess(
                    "OK",
                    "Cotización aceptada, rol actualizado y correo enviado correctamente");
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                _logger.LogError(ex, "Error al aceptar cotización.");
                return ServiceResult<string>.Failure("Error al aceptar la cotización.");
            }
        }

        public async Task<ServiceResult<string>> EnviarPdfCotizacion(EnviarPdfCotizacionDto dto)
        {
            var c = await _context.TbCotizaciones
                .Include(x => x.Usuario)
                .Include(x => x.CotizacionProductos).ThenInclude(cp => cp.Producto)
                .Include(x => x.CotizacionProductos).ThenInclude(cp => cp.Detalles).ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(x => x.IdCotizaciones == dto.IdCotizacion);

            if (c == null)
                return ServiceResult<string>.Failure("Cotización no encontrada.");

            // Construir DTO completo para generar el PDF
            var dtoFull = await GetCotizacionById(c.IdCotizaciones);
            if (dtoFull == null)
                return ServiceResult<string>.Failure("No se pudo construir la cotización para PDF.");

            // Generar PDF
            var pdfBytes = _pdf.GenerarPdfCotizacionConPartidas(dtoFull);

            // Destinatario
            var destinatario = string.IsNullOrWhiteSpace(dto.Destinatario)
                ? (c.Usuario?.Email ?? "")
                : dto.Destinatario.Trim();

            if (string.IsNullOrWhiteSpace(destinatario))
                return ServiceResult<string>.Failure("No hay correo de destino válido.");

            // Asunto / cuerpo
            var asunto = string.IsNullOrWhiteSpace(dto.Asunto)
                ? $"Cotización - {c.ClaveCotizacion}"
                : dto.Asunto.Trim();

            var cuerpo = string.IsNullOrWhiteSpace(dto.CuerpoHtml)
                ? $@"
            <h3>Tu cotización</h3>
            <p>Adjuntamos el PDF correspondiente a la cotización <b>{c.ClaveCotizacion}</b>.</p>"
                : dto.CuerpoHtml;

            try
            {
                await _emailService.EnviarCorreoAsync(
                    destinatario: destinatario,
                    asunto: asunto,
                    cuerpoHtml: cuerpo,
                    archivoAdjunto: pdfBytes,
                    nombreArchivo: $"Cotizacion-{c.ClaveCotizacion}.pdf"
                );

                return ServiceResult<string>.CreateSuccess("OK", "Correo enviado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de cotización.");
                return ServiceResult<string>.Failure("Error al enviar el correo.");
            }
        }

        public async Task<byte[]?> GenerarPdfBytes(int idCotizacion)
        {
            var dtoFull = await GetCotizacionById(idCotizacion);
            if (dtoFull == null) return null;

            var pdfBytes = _pdf.GenerarPdfCotizacionConPartidas(dtoFull);
            return pdfBytes;
        }

        private string GenerarClaveCotizacion()
            => "COT-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    }
}
