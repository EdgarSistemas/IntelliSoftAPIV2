using IntelliSoftAPIV2.Dtos.Pedidos;

namespace IntelliSoftAPIV2.Services.Pedidos
{
    public interface IPedidoService
    {
        Task<List<PedidoResponseDto>> ObtenerTodosAsync();
        Task<PedidoResponseDto?> ObtenerPorIdAsync(int id);
        Task<ServiceResult<string>> EliminarAsync(int id);
        Task<ServiceResult<string>> EstatusProcesoAsync(int id, int nuevoEstatus);
        Task<ServiceResult<string>> CompletarPedidoAsync(int id);

        Task<List<PedidoResponseDto>> ObtenerPorUsuarioAsync(string usuarioId);
    }
}
