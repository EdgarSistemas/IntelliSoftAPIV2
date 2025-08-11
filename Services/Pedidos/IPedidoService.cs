using IntelliSoftAPIV2.Dtos.Pedidos;

namespace IntelliSoftAPIV2.Services.Pedidos
{
    public interface IPedidoService
    {
        Task<List<PedidoResponseDto>> ObtenerTodosAsync();
        Task<PedidoResponseDto?> ObtenerPorIdAsync(int id);

        Task<ServiceResult<string>> CancelarAsync(int id);
        Task<ServiceResult<string>> ProcesarAsync(int id);
        Task<ServiceResult<string>> MarcarPagadoPorClienteAsync(int id);
        Task<ServiceResult<string>> FinalizarAsync(int id);

        Task<List<PedidoResponseDto>> ObtenerPorUsuarioAsync(string usuarioId);
    }
}
