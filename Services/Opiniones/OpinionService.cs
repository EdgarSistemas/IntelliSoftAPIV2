using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Opiniones;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Opiniones
{
    public class OpinionService : IOpinionService
    {
        private readonly AppDbContext _context;

        public OpinionService(AppDbContext context)
        {
            _context = context;
        }

        // Crear opinión (cliente)
        public async Task CrearAsync(OpinionCreateDto dto, string usuarioId)
        {
            var opinion = new TbOpinion
            {
                UsuarioId = usuarioId,
                ProductoId = dto.ProductoId,
                Calificacion = dto.Calificacion,
                Comentario = dto.Comentario,
                Fecha = DateTime.Now
            };

            _context.TbOpiniones.Add(opinion);
            await _context.SaveChangesAsync();
        }

        // Obtener todas (landing)
        public async Task<List<OpinionResponseDto>> ObtenerTodosAsync()
        {
            return await _context.TbOpiniones
                .Include(o => o.Usuario)
                .Include(o => o.Producto)
                .Include(o => o.Comentarios)
                .Select(o => new OpinionResponseDto
                {
                    IdOpinion = o.IdOpinion,
                    UsuarioId = o.UsuarioId,
                    UsuarioNombre = o.Usuario.Nombre + " " + o.Usuario.Apellidos,
                    ProductoId = o.ProductoId ?? 0,
                    ProductoNombre = o.Producto.Nombre,
                    Calificacion = o.Calificacion ?? 0,
                    Comentario = o.Comentario,
                    Fecha = o.Fecha,
                    ComentarioId = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => (int?)c.IdComentario)
                        .FirstOrDefault(),
                    ComentarioAdmin = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => c.Mensaje)
                        .FirstOrDefault(),
                    FechaComentarioAdmin = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => c.Fecha)
                        .FirstOrDefault()
                }).ToListAsync();
        }

        // Obtener por usuario (cliente)
        public async Task<List<OpinionResponseDto>> ObtenerPorUsuarioAsync(string usuarioId)
        {
            return await _context.TbOpiniones
                .Where(o => o.UsuarioId == usuarioId)
                .Include(o => o.Producto)
                .Include(o => o.Comentarios)
                .Select(o => new OpinionResponseDto
                {
                    IdOpinion = o.IdOpinion,
                    UsuarioId = o.UsuarioId,
                    UsuarioNombre = o.Usuario.Nombre + " " + o.Usuario.Apellidos,
                    ProductoId = o.ProductoId ?? 0,
                    ProductoNombre = o.Producto.Nombre,
                    Calificacion = o.Calificacion ?? 0,
                    Comentario = o.Comentario,
                    Fecha = o.Fecha,
                    ComentarioId = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => (int?)c.IdComentario)
                        .FirstOrDefault(),
                    ComentarioAdmin = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => c.Mensaje)
                        .FirstOrDefault(),
                    FechaComentarioAdmin = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => c.Fecha)
                        .FirstOrDefault()
                }).ToListAsync();
        }

        // Obtener por producto
        public async Task<List<OpinionResponseDto>> ObtenerPorProductoAsync(int productoId)
        {
            return await _context.TbOpiniones
                .Where(o => o.ProductoId == productoId)
                .Include(o => o.Usuario)
                .Include(o => o.Comentarios)
                .Select(o => new OpinionResponseDto
                {
                    IdOpinion = o.IdOpinion,
                    UsuarioId = o.UsuarioId,
                    UsuarioNombre = o.Usuario.Nombre + " " + o.Usuario.Apellidos,
                    ProductoId = o.ProductoId ?? 0,
                    ProductoNombre = o.Producto.Nombre,
                    Calificacion = o.Calificacion ?? 0,
                    Comentario = o.Comentario,
                    Fecha = o.Fecha,
                    ComentarioId = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => (int?)c.IdComentario)
                        .FirstOrDefault(),
                    ComentarioAdmin = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => c.Mensaje)
                        .FirstOrDefault(),
                    FechaComentarioAdmin = o.Comentarios
                        .OrderByDescending(c => c.Fecha)
                        .Select(c => c.Fecha)
                        .FirstOrDefault()
                }).ToListAsync();
        }

        // Editar opinión (cliente)
        public async Task<string> ActualizarAsync(int id, OpinionCreateDto dto, string usuarioId)
        {
            var opinion = await _context.TbOpiniones.FindAsync(id);

            if (opinion == null || opinion.UsuarioId != usuarioId)
                return "No tienes permiso para editar esta opinión.";

            opinion.Calificacion = dto.Calificacion;
            opinion.Comentario = dto.Comentario;
            await _context.SaveChangesAsync();

            return "Opinión actualizada correctamente.";
        }

        // Eliminar opinión (admin)
        public async Task<string> EliminarFisicaAsync(int id)
        {
            var opinion = await _context.TbOpiniones
                .Include(o => o.Comentarios)
                .FirstOrDefaultAsync(o => o.IdOpinion == id);

            if (opinion == null)
                return "Opinión no encontrada.";

            _context.TbComentarios.RemoveRange(opinion.Comentarios);
            _context.TbOpiniones.Remove(opinion);
            await _context.SaveChangesAsync();

            return "Opinión eliminada correctamente.";
        }

        // Crear comentario (admin)
        public async Task<string> CrearComentarioAsync(ComentarioCreateDto dto)
        {
            var opinion = await _context.TbOpiniones
                .Include(o => o.Comentarios)
                .FirstOrDefaultAsync(o => o.IdOpinion == dto.OpinionId);

            if (opinion == null)
                return "Opinión no encontrada.";

            var yaExiste = opinion.Comentarios.Any();
            if (yaExiste)
                return "La opinión ya tiene un comentario.";

            var comentario = new TbComentario
            {
                OpinionId = dto.OpinionId,
                Mensaje = dto.Mensaje,
                Fecha = DateTime.Now
            };

            _context.TbComentarios.Add(comentario);
            await _context.SaveChangesAsync();

            return "Comentario agregado correctamente.";
        }

        // Eliminar comentario (admin)
        public async Task<string> EliminarComentarioAsync(int idComentario)
        {
            var comentario = await _context.TbComentarios.FindAsync(idComentario);

            if (comentario == null)
                return "Comentario no encontrado.";

            _context.TbComentarios.Remove(comentario);
            await _context.SaveChangesAsync();

            return "Comentario eliminado correctamente.";
        }
    }
}
