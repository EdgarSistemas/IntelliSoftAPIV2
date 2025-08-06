using IntelliSoftAPIV2.Dtos.Opiniones;

namespace IntelliSoftAPIV2.Services.Opiniones
{
    public interface IOpinionService
    {
        Task CrearAsync(OpinionCreateDto dto, string usuarioId);
        Task<List<OpinionResponseDto>> ObtenerTodosAsync();
        Task<List<OpinionResponseDto>> ObtenerPorUsuarioAsync(string usuarioId);
        Task<List<OpinionResponseDto>> ObtenerPorProductoAsync(int productoId);
        Task<string> ActualizarAsync(int id, OpinionCreateDto dto, string usuarioId);
        Task<string> EliminarFisicaAsync(int id);
        Task<string> CrearComentarioAsync(ComentarioCreateDto dto);
        Task<string> EliminarComentarioAsync(int idComentario);
    }
}
