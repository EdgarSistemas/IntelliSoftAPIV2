using IntelliSoftAPIV2.Dtos.Opiniones;

namespace IntelliSoftAPIV2.Services.Opiniones
{
    public interface IOpinionService
    {
        Task CrearAsync(OpinionCreateDto dto, string usuarioId);
        Task<List<OpinionResponseDto>> ObtenerTodosAsync();
        Task<string> EliminarAsync(int id);
    }
}
