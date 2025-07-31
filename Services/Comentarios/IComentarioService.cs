using IntelliSoftAPIV2.Dtos.Comentarios;

namespace IntelliSoftAPIV2.Services.Comentarios
{
    public interface IComentarioService
    {
        Task CrearAsync(ComentarioCreateDto dto);
        Task<List<ComentarioResponseDto>> ObtenerTodosAsync();
        Task<ServiceResult<string>> EliminarAsync(int idComentario);
    }
}
