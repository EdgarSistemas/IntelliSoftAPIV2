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

        public async Task CrearAsync(OpinionCreateDto dto)
        {
            var opinion = new TbOpinion
            {
                UsuarioId = dto.UsuarioId,
                ProductoId = dto.ProductoId,
                Calificacion = dto.Calificacion,
                Comentario = dto.Comentario,
                Fecha = DateTime.Now,
            };

            _context.TbOpiniones.Add(opinion);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OpinionResponseDto>> ObtenerTodosAsync()
        {
            return await _context.TbOpiniones
                .Where(o => o.Estatus == 1)
                .Select(o => new OpinionResponseDto
                {
                    IdOpinion = o.IdOpinion,
                    UsuarioId = o.UsuarioId,
                    UsuarioNombre = o.Usuario.Nombre,
                    ProductoId = o.ProductoId ?? 0,
                    ProductoNombre = o.Producto.Nombre,
                    Calificacion = o.Calificacion ?? 0,
                    Comentario = o.Comentario,
                    Fecha = o.Fecha
                }).ToListAsync();
        }

        public async Task<string> EliminarAsync(int id)
        {
            var opinion = await _context.TbOpiniones.FindAsync(id);

            if (opinion == null)
                return "Opinión no encontrada.";

            if (opinion.Estatus == 0)
                return "La opinión ya estaba eliminada.";

            opinion.Estatus = 0;
            await _context.SaveChangesAsync();

            return "Opinión eliminada correctamente.";
        }
    }
}
