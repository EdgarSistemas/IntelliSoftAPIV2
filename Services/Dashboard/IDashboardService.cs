using IntelliSoftAPIV2.Dtos.Dashboard;

namespace IntelliSoftAPIV2.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<List<PedidosEstatusDto>> ObtenerPedidosEstatusAsync();
        Task<List<ProductoMasVendidoDto>> ObtenerProductosMasVendidosAsync();
        Task<List<ProductoOpinionDto>> ObtenerProductosMejorCalificadosAsync();
        Task<List<DistribucionOpinionDto>> ObtenerDistribucionOpinionesAsync();

    }
}
