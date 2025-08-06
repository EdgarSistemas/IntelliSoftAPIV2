using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Comentarios;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Comentarios
{
    public class ComentarioService : IComentarioService
    {
        private readonly AppDbContext _context;

        public ComentarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CrearAsync(ComentarioCreateDto dto)
        {
            var nuevoComentario = new TbComentario
            {
                Mensaje = dto.Mensaje,
                Fecha = DateTime.Now
            };

            _context.TbComentarios.Add(nuevoComentario);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ComentarioResponseDto>> ObtenerTodosAsync()
        {
            return await _context.TbComentarios
                .Where(c => c.Estatus == 1)
                .Select(c => new ComentarioResponseDto
                {
                    IdComentario = c.IdComentario,
                    Mensaje = c.Mensaje,
                    Fecha = c.Fecha
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<string>> EliminarAsync(int idComentario)
        {
            var comentario = await _context.TbComentarios.FirstOrDefaultAsync(c => c.IdComentario == idComentario);
            if (comentario == null || comentario.Estatus == 0)
                return ServiceResult<string>.Failure("Comentario no encontrado");

            comentario.Estatus = 0;
            await _context.SaveChangesAsync();
            return ServiceResult<string>.CreateSuccess("Comentario eliminado correctamente");
        }
    }
}
